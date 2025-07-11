namespace gmafffff.training.hotel.domain.PersonManagement.Contracts.Repositories;

public interface IPersonsRepository<TId> : IRepository<Person<TId>, TId>
    where TId : struct, IEquatable<TId>;