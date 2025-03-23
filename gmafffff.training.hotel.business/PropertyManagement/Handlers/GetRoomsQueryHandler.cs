using System.Collections.Immutable;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.PropertyManagement.Queries;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class GetRoomsQueryHandler(IHotelBlocksRepository<int, Guid> repo) : QueryDbHandler<GetRoomsQuery, RoomDto> {
    public override Task<IImmutableList<RoomDto>>
        CreateDbQuery(GetRoomsQuery query, CancellationToken cancel = default) {
        return repo.GetRoomsAsync(query.Filter, query.Pager, cancel);
    }
}