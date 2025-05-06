using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Contracts.Mappers;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class UpdateRoomCommandHandler(IHotelBlocksRepository<int, Guid> repo, IPropertyManagementMapper mapper)
    : BusinessCommandDbHandler<UpdateRoomCommand,
        HotelBlock<int, Guid>, int, IHotelBlocksRepository<int, Guid>,
        Room<Guid>, Room<Guid>>(repo) {
    protected override async Task<Fin<IList<Room<Guid>>>> LoadAsync(IHotelBlocksRepository<int, Guid> repo,
        CancellationToken cancel = default) {
        var found = (await repo
                .LoadWithRoomFilterAsync(predicate: room => room.Id == Command.Id, cancel)
                .ConfigureAwait(false))
            .SingleOrDefault()
            ?.Rooms
            .SingleOrDefault();

        return found is null
            ? AppErrorHelper.NewError(AppErrorCode.DbNotFound)
            : Fin<IList<Room<Guid>>>.Succ([found]);
    }

    protected override Task<Fin<IList<Room<Guid>>>> RunActionAsync(IList<Room<Guid>> loaded,
        CancellationToken cancel = default) {
        mapper.Update(Command.RoomUpdate, loaded[0]);
        return Task.FromResult(Fin<IList<Room<Guid>>>.Succ(loaded));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<Room<Guid>> result) {
        return result
            .Select(room => new UpdatedBusinessEvent<int>(room.Id, Command))
            .Cast<BusinessEvent>()
            .ToList();
    }
}