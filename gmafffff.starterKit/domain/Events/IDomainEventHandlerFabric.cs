namespace gmafffff.starterKit.Domain.Events;

/// <summary>
///     Чтобы не использовать ServiceLocator в потребляющем классе
/// </summary>
public interface IDomainEventHandlerFabric {
    IEnumerable<IDomainEventHandler> GetDomainEventHandlers(IDomainEvent @event);
}