using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.SampleModel.FakeDb;
using JetBrains.Annotations;

namespace gmafffff.training.hotel.infrastructure.data.tests.Tests;

[TestSubject(typeof(HotelBlocksRepository))]
public class HotelBlocksRepositoryTests : IClassFixture<SqliteHotelDbFixture> {
    private readonly FakeHotel _fakeHotel = new FakeHotelBuilder().Build();
    private readonly SqliteHotelDbFixture _sqliteHotelDbFixture;

    public HotelBlocksRepositoryTests(SqliteHotelDbFixture sqliteHotelDbFixture, ITestOutputHelper output) {
        _sqliteHotelDbFixture = sqliteHotelDbFixture;

        _sqliteHotelDbFixture.LogAction = output.WriteLine;
        // Для вывода лога в файл на рабочем столе
        // _sqliteHotelDbFixture.LogAction = _sqliteHotelDbFixture.DefaultFileLogStream.WriteLine;

        ClearDb().Wait();
    }

    private async Task ClearDb() {
        await new PersonsRepository(_sqliteHotelDbFixture.CreateDbContext()).DeleteBulkAsync(_ => true);
        await new AccommodationReportsRepository(_sqliteHotelDbFixture.CreateDbContext()).DeleteBulkAsync(_ => true);
        await new HotelBlocksRepository(_sqliteHotelDbFixture.CreateDbContext()).DeleteBulkAsync(_ => true);

        var repoInit = CreateRepo();
        repoInit.Add(_fakeHotel.Hotel);
        await repoInit.SaveChangesAsync();
    }

    private IHotelBlocksRepository<int, Guid> CreateRepo() {
        return new HotelBlocksRepository(_sqliteHotelDbFixture.CreateDbContext());
    }

    /// <summary>
    ///     Фильтрует номера корневой сущности по условию
    /// </summary>
    [Fact]
    public async Task FilterRootRoomsByCondition() {
        // Arrange
        var repoTest = CreateRepo();

        // Act 
        var onlyStandardRoom = (await repoTest
                .LoadWithRoomFilterAsync(room => room.RoomDetails.Type == RoomType.Standard))
            .SelectMany(hotel => hotel.Rooms)
            .Select(room => room.RoomDetails);

        // Assert
        onlyStandardRoom
            .Should().BeEquivalentTo(
                _fakeHotel.Hotel.Rooms
                    .Where(room => room.RoomDetails.Type == RoomType.Standard)
                    .Select(room => room.RoomDetails));
    }

    /// <summary>
    ///     Фильтрует номера в отеле по условию и проецирует их в dto
    /// </summary>
    [Fact]
    public async Task FilterRoomsByConditionAndProject() {
        // Arrange
        var repoTest = CreateRepo();

        // Act 
        var onlyStandardRoom = await repoTest
            .GetRoomsAsync(room => room.RoomDetails.Type == RoomType.Standard);

        // Assert        
        onlyStandardRoom
            .Should().BeEquivalentTo(
                _fakeHotel.Hotel.Rooms
                    .Where(room => room.RoomDetails.Type == RoomType.Standard));
    }

    /// <summary>
    ///     Считает количество номеров по условию
    /// </summary>
    [Fact]
    public async Task CountsNumberRoomsByCondition() {
        // Arrange
        var repoTest = CreateRepo();

        (await repoTest.CountRoomByAsync(Room<Guid>.IsFreeRoom))
            .Should().Be(
                _fakeHotel.Hotel.Rooms
                    .Where(Room<Guid>.IsFreeRoom.Compile())
                    .Count());
    }
}