using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Services.Repositories;

public interface IAccommodationReportsRepositoryFactory<TPersonId> :
    IRepositoryFactory<IAccommodationReportsRepository<TPersonId>, AccommodationReport<TPersonId>, int>
    where TPersonId : struct, IEquatable<TPersonId>;