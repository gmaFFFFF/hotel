using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class HotelBlocksRepositoryFactory(
    IDbContextFactory<HotelDbContext> dbContextFactory) :
    IHotelBlocksRepositoryFactory<int, Guid> {
    public IHotelBlocksRepository<int, Guid> CreateTransient() {
        return new HotelBlocksRepository(dbContextFactory.CreateDbContext());
    }
}