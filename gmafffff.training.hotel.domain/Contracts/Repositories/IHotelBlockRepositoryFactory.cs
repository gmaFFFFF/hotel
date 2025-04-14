using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Contracts.Repositories;

public interface IHotelBlocksRepositoryFactory<TId, TPersonId> :
    IRepositoryFactory<IHotelBlocksRepository<TId, TPersonId>, HotelBlock<TId, TPersonId>, TId>
    where TId : struct, IEquatable<TId>
    where TPersonId : struct, IEquatable<TPersonId>;