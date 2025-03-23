using System.Collections.Immutable;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Standard;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class RemoveRoomCommandHandler(IHotelBlocksRepository<int, Guid> repo)
    : BusinessCommandDbHandler<RemoveRoomsCommand, RemovedBusinessEvent<int>, Room<Guid>> {
    private IImmutableList<HotelBlock<int, Guid>>? _blocks;

    protected override async Task<Fin<Unit>> LoadAsync(CancellationToken cancel = default) {
        _blocks = await repo.LoadWithRoomFilterAsync(room => LastCommand!.Ids.Contains(room.Id));
        PreliminaryResult = _blocks
            .SelectMany(hotel => hotel.Rooms)
            .Where(room => LastCommand!.Ids.Contains(room.Id))
            .ToList();
        return Unit.Default;
    }

    protected override Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default) {
        repo.Delete<Room<Guid>, int>(PreliminaryResult);
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }

    protected override async Task<Fin<Unit>> SaveAsync(CancellationToken cancel = default) {
        await repo.SaveChangesAsync();
        return Unit.Default;
    }

    protected override void PackResultToEvent() {
        LastResult = PreliminaryResult
            .Select(room => new RemovedBusinessEvent<int>(room.Id, LastCommand!))
            .ToList();
    }
}