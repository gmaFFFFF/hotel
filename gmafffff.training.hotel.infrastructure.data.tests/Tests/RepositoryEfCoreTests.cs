using gmafffff.training.hotel.SampleModel.FakeDb;
using gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;

namespace gmafffff.training.hotel.infrastructure.data.tests.Tests;

public class RepositoryEfCoreTests : IClassFixture<SqliteDbFixture> {
    private readonly FakeHotel _fakeHotel = new FakeHotelBuilder().Build();
    private readonly SqliteDbFixture _sqliteDbFixture;

    public RepositoryEfCoreTests(SqliteDbFixture sqliteDbFixture, ITestOutputHelper output) {
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
    ///     Можно извлечь отсоединенные и присоединенные сущности
    /// </summary>
    [Fact]
    public async Task CanRetrieveDetachedAndAttachedEntities() {
        // Arrange
        using var repoInit = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoEditAttach = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoEditDetach = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());

        repoInit.Add(_fakeHotel.Reports);
        await repoInit.SaveChangesAsync();

        // Act 
        var reportAttach = (await repoEditAttach.LoadAllAsync())[0];
        reportAttach.Visitors.Add(default);
        await repoEditAttach.SaveChangesAsync();

        var reportDetach = (await repoEditDetach.GetAllDetachAsync()).Single(x => x.Id == reportAttach.Id);
        reportDetach.Visitors.Clear();
        await repoEditDetach.SaveChangesAsync();

        var reportResult = (await repoTest.GetAllDetachAsync()).Single(x => x.Id == reportAttach.Id);

        // Assert
        using var _ = new AssertionScope();
        reportDetach.Visitors.Should().BeEmpty();
        reportResult.Visitors.Should().Contain(default(Guid));
        reportResult.Visitors.Should().Equal(reportAttach.Visitors);
        reportResult.Visitors.Should().NotEqual(reportDetach.Visitors);
    }

    /// <summary>
    ///     Можно загружать сущности из БД по предикату
    /// </summary>
    [Fact]
    public async Task CanLoadEntitiesByPredicate() {
        // Arrange
        using var repoInit = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest1 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest2 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());

        repoInit.Add(_fakeHotel.Reports);
        await repoInit.SaveChangesAsync();
        var lostReport = _fakeHotel.Reports[new Random().Next(minValue: 0, _fakeHotel.Reports.Count)];

        // Act 
        var found = repoTest1.Load(r => r.Visitors.Contains(lostReport.Visitors.First()));
        var foundAsync = await repoTest2.LoadAsync(r => r.Visitors.Contains(lostReport.Visitors.First()));

        // Assert
        using var _ = new AssertionScope();
        found.Should().Contain(lostReport);
        foundAsync.Should().Contain(lostReport);
    }

    /// <summary>
    ///     Можно искать сущности локально и при необходимости в БД ключам
    /// </summary>
    [Fact]
    public async Task CanFindEntities() {
        // Arrange
        using var repoInit = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest1 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest2 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest3 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest4 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());

        repoInit.Add(_fakeHotel.Reports);
        await repoInit.SaveChangesAsync();

        var lostReports = _fakeHotel.Reports.Take(new Random().Next(minValue: 1, _fakeHotel.Reports.Count));
        var lostReportsIds = lostReports.Select(r => r.Id).ToArray();
        var testReportId = lostReportsIds[new Random().Next(minValue: 0, lostReportsIds.Length)];


        // Act 
        var testReport1 = repoTest1.Find(testReportId);
        var testReport2 = await repoTest2.FindAsync(testReportId);
        var found = repoTest1.Find(lostReportsIds);
        var foundAsync = await repoTest2.FindAsync(lostReportsIds);

        // Assert
        using var _ = new AssertionScope();
        found.Should().BeEquivalentTo(lostReports);
        foundAsync.Should().BeEquivalentTo(lostReports);

        testReport1.Should().BeSameAs(found.Single(x => x.Id == testReportId));
        testReport2.Should().BeSameAs(foundAsync.Single(x => x.Id == testReportId));
    }

    /// <summary>
    ///     Можно загружать сущности из БД
    /// </summary>
    [Fact]
    public async Task CanLoadEntities() {
        // Arrange
        using var repoInit = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest1 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest2 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest3 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest4 = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());

        repoInit.Add(_fakeHotel.Reports);
        await repoInit.SaveChangesAsync();

        var lostReports = _fakeHotel.Reports.Take(new Random().Next(minValue: 1, _fakeHotel.Reports.Count));
        var lostReportsIds = lostReports.Select(r => r.Id).ToArray();
        var testReportId = lostReportsIds[new Random().Next(minValue: 0, lostReportsIds.Length)];


        // Act 
        var testReport1 = repoTest1.Load(testReportId);
        var testReport2 = await repoTest2.LoadAsync(testReportId);
        var found = repoTest1.Load(lostReportsIds);
        var foundAsync = await repoTest2.LoadAsync(lostReportsIds);

        // Assert
        using var _ = new AssertionScope();
        found.Should().BeEquivalentTo(lostReports);
        foundAsync.Should().BeEquivalentTo(lostReports);

        testReport1.Should().Be(testReport2)
            .And.Be(lostReports.Single(r => r.Id == testReportId));
    }

    [Theory]
    [HotelAutodata]
    public async Task CanAddEntities(Person<Guid> person, IEnumerable<Person<Guid>> persons) {
        // Arrange
        using var repoInit = new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoTest = new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!);

        // Act
        repoInit.Add(person);
        repoInit.Add(persons);
        await repoInit.SaveChangesAsync();

        var saved = (await repoTest.GetAllDetachAsync()).Select(p => p.FullName);
        var excepted = persons.Append(person).Select(p => p.FullName);

        // Assert
        saved.Should().BeEquivalentTo(excepted);
    }

    [Theory]
    [HotelAutodata]
    public async Task CanDeleteChildEntities(HotelBlock<int, Guid> hotel, Room<Guid> room,
        IEnumerable<Room<Guid>> rooms) {
        var allRoom = rooms.Append(room);

        // Arrange
        using var repoInit = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoDel = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoTest = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);

        hotel.Rooms = allRoom.ToList();
        repoInit.Add(hotel);
        await repoInit.SaveChangesAsync();

        // Act
        repoDel.Delete<Room<Guid>, int>(room);
        repoDel.Delete<Room<Guid>, int>(rooms);
        await repoDel.SaveChangesAsync();

        var saved = await repoTest.LoadAsync(hotel.Id);
        // Assert
        saved.Rooms.Should().BeEmpty();
    }

    [Theory]
    [HotelAutodata]
    public async Task CanDeleteChildEntitiesById(HotelBlock<int, Guid> hotel, Room<Guid> room,
        IEnumerable<Room<Guid>> rooms) {
        var allRoom = rooms.Append(room);

        // Arrange
        using var repoInit = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoDel = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoTest = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);

        hotel.Rooms = allRoom.ToList();
        repoInit.Add(hotel);
        await repoInit.SaveChangesAsync();

        // Act
        repoDel.Delete<Room<Guid>, int>(room.Id);
        repoDel.Delete<Room<Guid>, int>(rooms.Select(r => r.Id));
        await repoDel.SaveChangesAsync();

        var saved = await repoTest.LoadAsync(hotel.Id);
        // Assert
        saved.Rooms.Should().BeEmpty();
    }

    [Theory]
    [HotelAutodata]
    public async Task CanDeleteEntitiesById(Person<Guid> person, IEnumerable<Person<Guid>> persons) {
        // Arrange
        using var repoInit = new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoDelete = new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoTest = new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!);

        // Act
        repoInit.Add(person);
        repoInit.Add(persons);
        await repoInit.SaveChangesAsync();

        repoDelete.Delete(person.Id);
        await repoDelete.SaveChangesAsync();
        var firstDelete = (await repoTest.GetAllDetachAsync()).Select(p => p.FullName);

        repoDelete.Delete(persons.Select(p => p.Id));
        await repoDelete.SaveChangesAsync();
        var lastDelete = await repoTest.GetAllDetachAsync();

        // Assert
        using var _ = new AssertionScope();
        firstDelete.Should().BeEquivalentTo(persons.Select(p => p.FullName));
        lastDelete.Should().BeEmpty();
    }

    [Theory]
    [HotelAutodata]
    public async Task CanUpdateEntity(Person<Guid> person, Person<Guid> personUpdate) {
        // Arrange
        using var repoInit = new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoMod = new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoTest = new PersonsRepository(_sqliteDbFixture.CreateDbContext(), null!);

        repoInit.Add(person);
        await repoInit.SaveChangesAsync();

        // Act
        personUpdate.Id = person.Id;
        repoMod.Update(personUpdate);
        await repoMod.SaveChangesAsync();

        var personSave = await repoMod.LoadAsync(person.Id);

        // Assert
        personSave.FullName.Should().Be(personUpdate.FullName);
    }
}