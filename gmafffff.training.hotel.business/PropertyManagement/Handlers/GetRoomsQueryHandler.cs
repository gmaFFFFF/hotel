using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.PropertyManagement.Queries;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class GetRoomsQueryHandler(IHotelBlocksRepository<int, Guid> repo) : QueryDbHandler<GetRoomsQuery, RoomDto> {
    public override async Task<IList<RoomDto>> CreateDbQuery(GetRoomsQuery query, 
        CancellationToken cancel = default) {
        return await repo
            .GetRoomsAsync(query.Filter, query.SortOrder, query.Pager, cancel)
            .ConfigureAwait(false);
    }
}