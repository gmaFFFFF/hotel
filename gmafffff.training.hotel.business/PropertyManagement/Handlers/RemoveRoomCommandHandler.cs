using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class RemoveRoomCommandHandler(IHotelBlocksRepository<int, Guid> repo)
    : BusinessCommandDbHandler<RemoveRoomsCommand, RemovedBusinessEvent<int>,
        HotelBlock<int, Guid>, int, IHotelBlocksRepository<int, Guid>,
        HotelBlock<int, Guid>, Room<Guid>>(repo) {
    protected override async Task<Fin<IList<HotelBlock<int, Guid>>>> LoadAsync(IHotelBlocksRepository<int, Guid> repo,
        CancellationToken cancel = default) {
        var load = await repo
            .LoadWithRoomFilterAsync(predicate: room => Command.Ids.Contains(room.Id), cancel)
            .ConfigureAwait(false);
        return load.ToArray();
    }

    protected override Task<Fin<IList<Room<Guid>>>> RunActionAsync(IList<HotelBlock<int, Guid>> loaded,
        CancellationToken cancel = default) {
        var del = loaded
            .SelectMany(hotel => hotel.Rooms)
            .Where(room => Command.Ids.Contains(room.Id))
            .ToArray();

        repo.Delete<Room<Guid>, int>(del);
        return Task.FromResult(Fin<IList<Room<Guid>>>.Succ(del));
    }

    protected override IList<RemovedBusinessEvent<int>> PackResultToEvent(IList<Room<Guid>> result) {
        return result
            .Select(room => new RemovedBusinessEvent<int>(room.Id, Command))
            .ToList();
    }
}