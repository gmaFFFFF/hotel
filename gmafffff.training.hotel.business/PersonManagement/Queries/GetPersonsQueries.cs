using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.business.PersonManagement.Queries;

public record GetPersonsQueryByDto<TDto>(
    Expression<Func<TDto, bool>> Filter,
    Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : ReadDbQuery<TDto>(Filter, SortOrder, Pager);

public record GetPersonsQueryByEntity<TDto>(
    Expression<Func<Person<Guid>, bool>> Filter,
    Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : ReadDbQuery<Person<Guid>, TDto>(Filter, SortOrder, Pager);