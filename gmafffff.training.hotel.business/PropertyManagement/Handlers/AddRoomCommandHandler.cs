using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Contracts.Mappers;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class AddRoomCommandHandler(IHotelBlocksRepository<int, Guid> repo, IPropertyManagementMapper mapper)
    : BusinessCommandDbHandler<AddRoomCommand,
        HotelBlock<int, Guid>, int, IHotelBlocksRepository<int, Guid>,
        HotelBlock<int, Guid>, Room<Guid>>(repo) {
    protected override async Task<Fin<IList<HotelBlock<int, Guid>>>> LoadAsync(IHotelBlocksRepository<int, Guid> repo,
        CancellationToken cancel = default) {
        var loaded = await repo
            .LoadOnlyRootAsync(spec: hotel => hotel.Id == Command.Room.HotelBlockId, cancel)
            .ConfigureAwait(false);

        return loaded.Count == 0
            ? AppErrorHelper.NewError(AppErrorCode.DbNotFound)
            : loaded.ToArray();
    }

    protected override Task<Fin<IList<Room<Guid>>>> RunActionAsync(IList<HotelBlock<int, Guid>> loaded,
        CancellationToken cancel = default) {
        var newRoom = mapper.Map(Command.Room);
        loaded[0].Rooms = [newRoom];
        return Task.FromResult(Fin<IList<Room<Guid>>>.Succ([newRoom]));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<Room<Guid>> result) {
        return result
            .Select(room => new CreatedBusinessEvent<int>(room.Id, Command))
            .Cast<BusinessEvent>()
            .ToList();
    }
}