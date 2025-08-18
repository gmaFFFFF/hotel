using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.business.PersonManagement.Queries;

public record GetPersonsQueryByDto<TPersonDto>(
    Expression<Func<TPersonDto, bool>> Filter,
    Func<IQueryable<TPersonDto>, IOrderedQueryable<TPersonDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : ReadDbQuery<TPersonDto>(Filter, SortOrder, Pager);

public record GetPersonsQueryByEntity<TPersonDto>(
    Expression<Func<Person<Guid>, bool>> Filter,
    Func<IQueryable<TPersonDto>, IOrderedQueryable<TPersonDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : ReadDbQuery<Person<Guid>, TPersonDto>(Filter, SortOrder, Pager);