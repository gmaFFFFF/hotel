using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class PersonsRepository(HotelDbContext context) :
    Repository<Person<Guid>, Guid>(context),
    IPersonsRepository<Guid>;