using gmafffff.training.hotel.SampleModel.FakeDb;
using gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;
using JetBrains.Annotations;

namespace gmafffff.training.hotel.infrastructure.data.tests.Tests;

[TestSubject(typeof(HotelBlocksRepository))]
public class SettlementTests : IClassFixture<SqliteDbFixture> {
    private readonly FakeHotel _fakeHotel = new FakeHotelBuilder().Build();
    private readonly SqliteDbFixture _sqliteDbFixture;

    public SettlementTests(SqliteDbFixture sqliteDbFixture, ITestOutputHelper output) {
        _sqliteDbFixture = sqliteDbFixture;

        _sqliteDbFixture.LogAction = output.WriteLine;
        // Для вывода лога в файл на рабочем столе
        // _sqliteDbFixture.LogAction = _sqliteDbFixture.DefaultFileLogStream.WriteLine;
        ClearDb().Wait();
    }

    private async Task ClearDb() {
        await new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!).DeleteBulkAsync(_ => true);
        await new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext()).DeleteBulkAsync(_ => true);
        await new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!).DeleteBulkAsync(_ => true);
    }

    /// <summary>
    ///     Параллельное изменение данных о посещении выбрасывает исключение
    /// </summary>
    [Theory]
    [HotelAutodata]
    public async Task ConcurrentModificationOfVisitorThrowsException(Person<Guid> person) {
        // Arrange
        var initRepo = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        var settledRepo = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        var firstRepo = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        var parallelRepo = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);

        var testRepo = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);

        // Инициализация хранилища
        initRepo.Add(_fakeHotel.Hotel);
        await initRepo.SaveChangesAsync();

        // Находим свободный номер и заселяем в него человека
        var initHotel = (await settledRepo.LoadWithRoomFilterAsync(Room<Guid>.IsFreeRoom))[0];
        var testRoom = initHotel.Rooms.First();
        initHotel.SettledIn([person.Id], testRoom);
        await settledRepo.SaveChangesAsync();

        // Запрашиваем номер в первом соединении
        var firstQueryHotel =
            (await firstRepo.LoadWithRoomFilterAsync(Room<Guid>.FilterByNumber(testRoom.RoomDetails.Number)))
            .Single(h => h.Id == initHotel.Id);
        var firstQueryRoom = firstQueryHotel.Rooms.Single();

        // Запрашиваем номер в параллельном соединении
        var parallelQueryHotel =
            (await parallelRepo.LoadWithRoomFilterAsync(Room<Guid>.FilterByNumber(testRoom.RoomDetails.Number)))
            .Single(h => h.Id == initHotel.Id);
        var parallelQueryRoom = parallelQueryHotel.Rooms.Single();

        // Параллельно изменяем информацию о проживании
        firstQueryRoom.Visit.DepartureDatePlanned = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
        parallelQueryRoom.Visit.TariffDetails = parallelQueryRoom.Visit.TariffDetails with { Value = 3141.59m };


        // Act
        var settlementAct = async () => await firstRepo.SaveChangesAsync();
        var parallelAct = async () => await parallelRepo.SaveChangesAsync();

        // Assert
        using var _ = new AssertionScope();
        await settlementAct.Should().NotThrowAsync();
        await parallelAct.Should().ThrowAsync<DbUpdateConcurrencyException>();

        (await testRepo.LoadWithRoomFilterAsync(Room<Guid>.FilterByNumber(testRoom.RoomDetails.Number)))
            .Single(h => h.Id == initHotel.Id)
            .Rooms.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(firstQueryRoom);
    }
}