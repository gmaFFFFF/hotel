using gmafffff.training.hotel.domain.Contracts.Mappers;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class HotelBlocksRepositoryFactory(
    IDbContextFactory<HotelDbContext> dbContextFactory,
    IPropertyManagementMapper propertyManagementMapper) :
    IHotelBlocksRepositoryFactory<int, Guid> {
    public IHotelBlocksRepository<int, Guid> CreateTransient() {
        return new HotelBlocksRepository(dbContextFactory.CreateDbContext(), propertyManagementMapper);
    }
}