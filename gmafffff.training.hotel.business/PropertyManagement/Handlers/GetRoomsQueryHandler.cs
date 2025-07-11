using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.business.PropertyManagement.Dto;
using gmafffff.training.hotel.business.PropertyManagement.Queries;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class GetRoomsQueryHandler(
    IHotelBlocksRepository<int, Guid> repo,
    IEntityMapperForwardExpression<Room<Guid>, int, RoomDto> mapper)
    : QueryDbHandler<GetRoomsQuery, RoomDto> {
    protected override async Task<IList<RoomDto>> RunDbQueryAsync(GetRoomsQuery query,
        CancellationToken cancel = default) {
        return await repo
            .GetRoomsDtoAsync(mapper.EntityToDto, query.Filter, query.SortOrder, query.Pager, cancel)
            .ConfigureAwait(false);
    }
}