using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Contracts.Repositories;

public interface IPersonsRepositoryFactory<TPersonId> :
    IRepositoryFactory<IPersonsRepository<TPersonId>, Person<TPersonId>, TPersonId>
    where TPersonId : struct, IEquatable<TPersonId>;