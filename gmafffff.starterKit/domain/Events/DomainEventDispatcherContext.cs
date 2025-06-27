namespace gmafffff.starterKit.Domain.Events;

/// <summary>
///     Контекст обработки событий домена
/// </summary>
/// <param name="UnhandledEvents">Необработанные события</param>
/// <param name="ProcessedEvents">Обработанные события</param>
/// <param name="ServiceProvider">Контейнер сервисов</param>
public record DomainEventDispatcherContext(
    IEnumerable<IDomainEvent> UnhandledEvents,
    IEnumerable<IDomainEvent> ProcessedEvents,
    IServiceProvider ServiceProvider);