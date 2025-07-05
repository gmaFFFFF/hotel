using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class PersonsRepositoryFactory(
    IDbContextFactory<HotelDbContext> contextFactory) :
    IPersonsRepositoryFactory<Guid> {
    public IPersonsRepository<Guid> CreateTransient() {
        return new PersonsRepository(contextFactory.CreateDbContext());
    }
}