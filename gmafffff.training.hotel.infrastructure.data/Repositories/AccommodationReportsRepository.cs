using gmafffff.starterKit.Db;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class AccommodationReportsRepository(HotelDbContext context) :
    RepositoryEfCore<AccommodationReport<Guid>, int>(context),
    IAccommodationReportsRepository<Guid> { }