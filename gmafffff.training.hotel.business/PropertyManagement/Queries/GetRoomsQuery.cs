using gmafffff.training.hotel.domain.Model;
using Query = gmafffff.starterKit.Messaging.Query;

namespace gmafffff.training.hotel.business.PropertyManagement.Queries;

public record GetRoomsQuery(Expression<Func<Room<Guid>, bool>> Filter, (uint pageNum, uint pageSize)? Pager = null)
    : Query(Pager);