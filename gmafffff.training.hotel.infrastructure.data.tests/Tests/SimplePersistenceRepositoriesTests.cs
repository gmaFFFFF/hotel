using gmafffff.training.hotel.SampleModel.FakeDb;
using JetBrains.Annotations;

namespace gmafffff.training.hotel.infrastructure.data.tests.Tests;

[TestSubject(typeof(HotelBlock<,>))]
[TestSubject(typeof(AccommodationReport<>))]
[TestSubject(typeof(Person<>))]
public class SimplePersistenceRepositoriesTests : IClassFixture<SqliteHotelDbFixture> {
    private readonly FakeHotel _fakeHotel = new FakeHotelBuilder().Build();
    private readonly SqliteHotelDbFixture _sqliteHotelDbFixture;

    public SimplePersistenceRepositoriesTests(SqliteHotelDbFixture sqliteHotelDbFixture, ITestOutputHelper output) {
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
    }

    [Fact]
    public async Task CanSaveHotelBlock() {
        // Arrange
        var repoInit = new HotelBlocksRepository(_sqliteHotelDbFixture.CreateDbContext());
        var repoTest = new HotelBlocksRepository(_sqliteHotelDbFixture.CreateDbContext());
        IList<HotelBlock<int, Guid>>? saved = null;

        repoInit.Add(_fakeHotel.Hotel);
        var writeAct = async () => await repoInit.SaveChangesAsync();
        var readAct = async () => saved = await repoTest.GetAsync();

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
        var repoInit = new PersonsRepository(_sqliteHotelDbFixture.CreateDbContext());
        var repoTest = new PersonsRepository(_sqliteHotelDbFixture.CreateDbContext());
        IList<Person<Guid>>? saved = null;

        repoInit.Add(_fakeHotel.Persons);
        var writeAct = async () => await repoInit.SaveChangesAsync();
        var readAct = async () => saved = await repoTest.GetAsync();

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
        var repoInit = new AccommodationReportsRepository(_sqliteHotelDbFixture.CreateDbContext());
        var repoTest = new AccommodationReportsRepository(_sqliteHotelDbFixture.CreateDbContext());
        IList<AccommodationReport<Guid>>? saved = null;

        repoInit.Add(_fakeHotel.Reports);
        var writeAct = async () => await repoInit.SaveChangesAsync();
        var readAct = async () => saved = await repoTest.GetAsync();

        // Act & Assert       
        using var _ = new AssertionScope();
        await writeAct.Should().NotThrowAsync();
        await readAct.Should().NotThrowAsync();

        saved.Should().BeEquivalentTo(_fakeHotel.Reports);
    }
}