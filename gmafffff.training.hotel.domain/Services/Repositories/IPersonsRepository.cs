using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Services.Repositories;

public interface IPersonsRepository<TId> : IRepository<Person<TId>, TId>
    where TId : struct, IEquatable<TId> { }