using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.business.PropertyManagement.Queries;

public record GetRoomsQuery<TDto>(
    Expression<Func<Room<Guid>, bool>> Filter,
    Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : ReadDbQuery<Room<Guid>, TDto>(Filter, SortOrder, Pager);