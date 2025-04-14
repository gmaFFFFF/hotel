using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.sampleModel.FakeModel;
using gmafffff.training.hotel.SampleModel.FakeServices;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.Fixtures;

public class TestContext : IDisposable {
    private readonly Lazy<StreamWriter> _logStream = new(() =>
        new StreamWriter(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ef_core.log"),
            append: true));

    protected readonly FakeHotel FakeHotel = new FakeHotelBuilder().Build();
    protected readonly FakeServiceProvider FakeServiceProvider;
    protected readonly IHotelBlocksRepository<int, Guid> HotelRepo;
    protected readonly IHotelBlocksRepositoryFactory<int, Guid> HotelRepoFactory;
    protected readonly ITestOutputHelper Output;
    protected readonly IPersonsRepository<Guid> PersonRepo;
    protected readonly IPersonsRepositoryFactory<Guid> PersonRepoFactory;
    protected readonly IServiceScope Scope;

    protected bool DisableLog = true;
    protected bool LogToFile = false;

    public TestContext(ITestOutputHelper output) {
        Output = output;
        FakeServiceProvider = new FakeServiceProvider(LogAction);
        HotelRepoFactory = FakeServiceProvider.Instance.GetRequiredService<IHotelBlocksRepositoryFactory<int, Guid>>();
        PersonRepoFactory = FakeServiceProvider.Instance.GetRequiredService<IPersonsRepositoryFactory<Guid>>();
        Scope = FakeServiceProvider.Instance.CreateScope();
        HotelRepo = Scope.ServiceProvider.GetRequiredService<IHotelBlocksRepository<int, Guid>>();
        PersonRepo = Scope.ServiceProvider.GetRequiredService<IPersonsRepository<Guid>>();

        ClearDb().Wait();
    }

    public void Dispose() {
        Scope.Dispose();
        FakeServiceProvider.Dispose();

        if (_logStream.IsValueCreated)
            _logStream.Value.Close();
    }

    private void LogAction(string message) {
        if (DisableLog) return;

        if (LogToFile) _logStream.Value.WriteLine(message);
        else Output.WriteLine(message);
    }

    private async Task ClearDb() {
        using var scope = FakeServiceProvider.Instance.CreateScope();
        var hotelRepo = scope.ServiceProvider.GetRequiredService<IHotelBlocksRepository<int, Guid>>();
        var personRepo = scope.ServiceProvider.GetRequiredService<IPersonsRepository<Guid>>();
        DisableLog = true;
        await hotelRepo.DeleteBulkAsync(_ => true);
        await personRepo.DeleteBulkAsync(_ => true);
        hotelRepo.Add(FakeHotel.Hotel);
        personRepo.Add(FakeHotel.Persons);
        await hotelRepo.SaveChangesAsync();
        await personRepo.SaveChangesAsync();

        DisableLog = false;
    }
}