using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Contracts.Repositories;

public interface IAccommodationReportsRepository<TPersonId> : IRepository<AccommodationReport<TPersonId>, int>
    where TPersonId : struct, IEquatable<TPersonId> { }