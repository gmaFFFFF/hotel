using gmafffff.starterKit.Domain.Events;
using gmafffff.training.hotel.domain.SettlementManagement.DomainEvents;
using LanguageExt;

namespace gmafffff.training.hotel.domain.SettlementManagement.DomainEventHandlers;

/// <summary>
///     Событие выселения ставит номер в очередь на уборку
/// </summary>
public class MoveOutQueueClean : IDomainEventHandler<MoveOutDomainEvent> {
    public Task<Fin<Unit>> HandleAsync(MoveOutDomainEvent @event, DomainEventDispatcherContext context,
        CancellationToken cancel = default) {
        if (@event.Sender.IsClean ?? throw new NullReferenceException())
            @event.Sender.RoomCleanings.Add(new RoomCleaning { PutInLine = DateTime.UtcNow });
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }
}