using gmafffff.training.hotel.domain.Contracts.Mappers;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.infrastructure.mapper.Required;
using gmafffff.training.hotel.SampleModel.FakeDb;

namespace gmafffff.training.hotel.infrastructure.data.tests.Tests;

public class HotelBlocksRepositoryTests : IClassFixture<SqliteDbFixture> {
    private readonly FakeHotel _fakeHotel = new FakeHotelBuilder().Build();
    private readonly IPropertyManagementMapper _mapper = new PropertyManagementMapperMapster();
    private readonly SqliteDbFixture _sqliteDbFixture;

    public HotelBlocksRepositoryTests(SqliteDbFixture sqliteDbFixture, ITestOutputHelper output) {
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

        using var repoInit = CreateRepo();
        repoInit.Add(_fakeHotel.Hotel);
        await repoInit.SaveChangesAsync();
    }

    private IHotelBlocksRepository<int, Guid> CreateRepo() {
        return new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), _mapper);
    }

    /// <summary>
    ///     Фильтрует номера корневой сущности по условию
    /// </summary>
    [Fact]
    public async Task FilterRootRoomsByCondition() {
        // Arrange
        using var repoTest = CreateRepo();

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
        using var repoTest = CreateRepo();

        // Act 
        var onlyStandardRoom = await repoTest
            .GetRoomsAsync(room => room.RoomDetails.Type == RoomType.Standard);

        // Assert        
        onlyStandardRoom
            .Should().BeEquivalentTo(
                _fakeHotel.Hotel.Rooms
                    .Where(room => room.RoomDetails.Type == RoomType.Standard)
                    .Select(room => _mapper.Map(room)));
    }

    /// <summary>
    ///     Считает количество номеров по условию
    /// </summary>
    [Fact]
    public async Task CountsNumberRoomsByCondition() {
        // Arrange
        using var repoTest = CreateRepo();

        (await repoTest.CountRoomByAsync(Room<Guid>.IsFreeRoom))
            .Should().Be(
                _fakeHotel.Hotel.Rooms
                    .Where(Room<Guid>.IsFreeRoom.Compile())
                    .Count());
    }
}