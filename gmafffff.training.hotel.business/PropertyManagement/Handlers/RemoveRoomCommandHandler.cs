using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class RemoveRoomCommandHandler(
    IHotelBlocksRepository<int, Guid> repo,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<RemoveRoomCommandHandler>? logger = null)
    : BusinessCommandDbHandler<RemoveRoomsCommand,
        HotelBlock<int, Guid>, int, IHotelBlocksRepository<int, Guid>,
        HotelBlock<int, Guid>, Room<Guid>>(repo, domainEventDispatcher, logger: logger) {
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

    protected override IList<BusinessEvent> PackResultToEvent(IList<Room<Guid>> result) {
        return result
            .Select(room => new DeletedBusinessEvent<int>(room.Id, Command))
            .Cast<BusinessEvent>()
            .ToList();
    }
}