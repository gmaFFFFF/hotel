using System.Collections.Immutable;
using gmafffff.starterKit.Domain;
using gmafffff.training.hotel.domain.Dto.PersonManagement;

namespace gmafffff.training.hotel.domain.Services.Repositories;

public interface IPersonsRepository<TId> : IRepository<Person<TId>, TId>
    where TId : struct, IEquatable<TId> {
    /// <summary>
    ///     Возвращает DTO
    /// </summary>
    /// <param name="predicate">фильтр персон</param>
    /// <param name="pager">страничная разбивка</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    Task<IImmutableList<PersonDto>> GetPersonsAsync(Expression<Func<Person<Guid>, bool>> predicate,
        (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default);
}