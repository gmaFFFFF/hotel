using gmafffff.starterKit.Di;
using gmafffff.training.hotel.business.PropertyManagement.BusinessConstraintsChecks;
using gmafffff.training.hotel.business.PropertyManagement.Handlers;
using gmafffff.training.hotel.business.SettlementManagement.Validations;
using gmafffff.training.hotel.domain.Validation;
using gmafffff.training.hotel.infrastructure.data.Repositories;
using gmafffff.training.hotel.infrastructure.data.Sessions;
using gmafffff.training.hotel.infrastructure.mapper.Required;
using gmafffff.training.hotel.SampleModel.FakeDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace gmafffff.training.hotel.SampleModel.FakeServices;

public class FakeServiceProvider : IDisposable {
    public readonly ServiceProvider Instance;

    public FakeServiceProvider(Action<string>? logAction = null) {
        var sqliteDbFixture = new SqliteDbFixture();
        sqliteDbFixture.LogAction = logAction ?? (_ => { });
        // Для вывода лога в файл на рабочем столе
        // _sqliteDbFixture.LogAction = _sqliteDbFixture.DefaultFileLogStream.WriteLine;

        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddScoped<HotelDbContext>(_ => sqliteDbFixture.CreateDbContext());
        ServiceCollection.AddSingleton<IDbContextFactory<HotelDbContext>>(_ => {
            var factory = Substitute.For<IDbContextFactory<HotelDbContext>>();
            factory.CreateDbContext().Returns(_ => sqliteDbFixture.CreateDbContext());
            return factory;
        });

        ServiceCollection.AddRepositories(typeof(HotelBlocksRepository).Assembly);

        ServiceCollection.AddValidotValidators([typeof(RoomSpec).Assembly, typeof(SettleInCommandSpec).Assembly]);
        ServiceCollection.AddBusinessConstraintsChecks(typeof(RoomNumberMustUnique).Assembly);
        ServiceCollection.AddEntityMappersWithConfig(typeof(IPropertyManagementMapperMapster).Assembly);
        ServiceCollection.AddBusinessCommandDbHandlers(typeof(AddRoomCommandHandler).Assembly);
        ServiceCollection.AddQueryHandlers(typeof(GetRoomsQueryHandler).Assembly);

        Instance = ServiceCollection.BuildServiceProvider(new ServiceProviderOptions {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
    }

    private ServiceCollection ServiceCollection { get; }

    public void Dispose() {
        Instance.Dispose();
    }
}