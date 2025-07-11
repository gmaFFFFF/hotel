namespace gmafffff.training.hotel.domain.SettlementManagement.Contracts.Repositories;

public interface IAccommodationReportsRepository<TPersonId> : IRepository<AccommodationReport<TPersonId>, int>
    where TPersonId : struct, IEquatable<TPersonId> { }