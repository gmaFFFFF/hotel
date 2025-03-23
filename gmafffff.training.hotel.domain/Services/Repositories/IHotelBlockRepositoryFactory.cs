using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Services.Repositories;

public interface IHotelBlocksRepositoryFactory<TId, TPersonId> :
    IRepositoryFactory<IHotelBlocksRepository<TId, TPersonId>, HotelBlock<TId, TPersonId>, TId>
    where TId : struct, IEquatable<TId>
    where TPersonId : struct, IEquatable<TPersonId>;