namespace gmafffff.training.hotel.domain.Services.Repositories;

public interface IAccommodationReportsRepository<TPersonId> : IRepository<AccommodationReport<TPersonId>, int>
    where TPersonId : struct, IEquatable<TPersonId> { }