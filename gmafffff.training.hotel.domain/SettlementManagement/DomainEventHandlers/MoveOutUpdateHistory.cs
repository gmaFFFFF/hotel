using gmafffff.starterKit.Domain.Events;
using gmafffff.training.hotel.domain.PersonManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.SettlementManagement.DomainEvents;
using LanguageExt;

namespace gmafffff.training.hotel.domain.SettlementManagement.DomainEventHandlers;

/// <summary>
///     Событие выселения обновляет статистику посещения гостиницы у посетителя
/// </summary>
public class MoveOutUpdateHistory(IPersonsRepository<Guid> repository) : IDomainEventHandler<MoveOutDomainEvent> {
    public async Task<Fin<Unit>> HandleAsync(MoveOutDomainEvent @event, DomainEventDispatcherContext context,
        CancellationToken cancel = default) {
        var visitors = await repository.LoadAsync(@event.Visit.Visitors, cancel);
        foreach (var visitor in visitors)
            visitor.History = visitor.History with {
                Count = visitor.History.Count + 1,
                Duration = visitor.History.Duration +
                           DateTime.Today.Subtract(@event.Visit.ArrivalDate.ToDateTime(TimeOnly.MinValue)).Days
            };

        return Fin<Unit>.Succ(Unit.Default);
    }
}