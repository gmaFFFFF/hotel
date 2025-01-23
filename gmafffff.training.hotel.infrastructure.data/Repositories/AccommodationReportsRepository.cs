namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class AccommodationReportsRepository(DbContext context) :
    RepositoryEfCore<AccommodationReport<Guid>, int>(context),
    IAccommodationReportsRepository<Guid> { }