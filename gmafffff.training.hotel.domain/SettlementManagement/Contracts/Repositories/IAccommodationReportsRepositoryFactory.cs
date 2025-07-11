namespace gmafffff.training.hotel.domain.SettlementManagement.Contracts.Repositories;

public interface IAccommodationReportsRepositoryFactory<TPersonId> :
    IRepositoryFactory<IAccommodationReportsRepository<TPersonId>, AccommodationReport<TPersonId>, int>
    where TPersonId : struct, IEquatable<TPersonId>;