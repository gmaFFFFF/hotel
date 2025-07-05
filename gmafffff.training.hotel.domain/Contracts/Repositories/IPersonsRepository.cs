using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Contracts.Repositories;

public interface IPersonsRepository<TId> : IRepository<Person<TId>, TId>
    where TId : struct, IEquatable<TId>;