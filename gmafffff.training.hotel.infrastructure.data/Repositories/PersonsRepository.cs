using gmafffff.starterKit.Db;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class PersonsRepository(DbContext context) :
    RepositoryEfCore<Person<Guid>, Guid>(context),
    IPersonsRepository<Guid> { }