using gmafffff.training.hotel.business.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.PropertyManagement.Queries;

public record GetRoomsQuery(
    Expression<Func<Room<Guid>, bool>> Filter,
    Func<IQueryable<RoomDto>, IOrderedQueryable<RoomDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : Query<RoomDto>(SortOrder, Pager);