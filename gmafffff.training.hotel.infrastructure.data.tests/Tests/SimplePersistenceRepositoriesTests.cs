namespace gmafffff.training.hotel.infrastructure.data.tests.Tests;

public class SimplePersistenceRepositoriesTests : IClassFixture<SqliteDbFixture> {
    private readonly FakeHotel _fakeHotel = new FakeHotelBuilder().Build();
    private readonly SqliteDbFixture _sqliteDbFixture;

    public SimplePersistenceRepositoriesTests(SqliteDbFixture sqliteDbFixture, ITestOutputHelper output) {
        _sqliteDbFixture = sqliteDbFixture;

        _sqliteDbFixture.LogAction = output.WriteLine;
        // Для вывода лога в файл на рабочем столе
        // _sqliteDbFixture.LogAction = _sqliteDbFixture.DefaultFileLogStream.WriteLine;

        ClearDb().Wait();
    }

    private async Task ClearDb() {
        await new PersonsRepository(_sqliteDbFixture.CreateDbContext()).DeleteBulkAsync(_ => true);
        await new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext()).DeleteBulkAsync(_ => true);
        await new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!).DeleteBulkAsync(_ => true);
    }

    [Fact]
    public async Task CanSaveHotelBlock() {
        // Arrange
        using var repoInit = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        using var repoTest = new HotelBlocksRepository(_sqliteDbFixture.CreateDbContext(), null!);
        IImmutableList<HotelBlock<int, Guid>>? saved = null;

        repoInit.Add(_fakeHotel.Hotel);
        var writeAct = async () => await repoInit.SaveChangesAsync();
        var readAct = async () => saved = await repoTest.GetAllDetachAsync();

        // Act & Assert       
        using var _ = new AssertionScope();
        await writeAct.Should().NotThrowAsync();
        await readAct.Should().NotThrowAsync();

        saved.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(_fakeHotel.Hotel);
    }

    [Fact]
    public async Task CanSavePersons() {
        // Arrange
        using var repoInit = new PersonsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest = new PersonsRepository(_sqliteDbFixture.CreateDbContext());
        IImmutableList<Person<Guid>>? saved = null;

        repoInit.Add(_fakeHotel.Persons);
        var writeAct = async () => await repoInit.SaveChangesAsync();
        var readAct = async () => saved = await repoTest.GetAllDetachAsync();

        // Act & Assert       
        using var _ = new AssertionScope();
        await writeAct.Should().NotThrowAsync();
        await readAct.Should().NotThrowAsync();


        // Для ускорения теста сравнение графа объектов заменено
        // saved.Should().BeEquivalentTo(_fakeHotel.Persons);
        saved.OrderBy(p => p.Id).Select(p => (p.Id, p.FullName)).Should().Equal(
            _fakeHotel.Persons.OrderBy(p => p.Id).Select(p => (p.Id, p.FullName)));
    }

    [Fact]
    public async Task CanSaveAccommodationReports() {
        // Arrange
        using var repoInit = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        using var repoTest = new AccommodationReportsRepository(_sqliteDbFixture.CreateDbContext());
        IImmutableList<AccommodationReport<Guid>>? saved = null;

        repoInit.Add(_fakeHotel.Reports);
        var writeAct = async () => await repoInit.SaveChangesAsync();
        var readAct = async () => saved = await repoTest.GetAllDetachAsync();

        // Act & Assert       
        using var _ = new AssertionScope();
        await writeAct.Should().NotThrowAsync();
        await readAct.Should().NotThrowAsync();

        saved.Should().BeEquivalentTo(_fakeHotel.Reports);
    }
}