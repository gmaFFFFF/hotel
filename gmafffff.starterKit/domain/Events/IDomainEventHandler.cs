using System.ComponentModel;
using LanguageExt;

namespace gmafffff.starterKit.Domain.Events;

/// <summary>
///     Обработчик событий домена. Маркерный интерфейс, предназначенный для использования внутри библиотеки
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IDomainEventHandler {
    Task<Fin<Unit>> HandleAsync(IDomainEvent @event, DomainEventDispatcherContext context,
        CancellationToken cancel = default);
}

/// <summary>
///     Обработчик событий домена
/// </summary>
public interface IDomainEventHandler<in TDomainEvent> : IDomainEventHandler
    where TDomainEvent : IDomainEvent {
    async Task<Fin<Unit>> IDomainEventHandler.HandleAsync(IDomainEvent @event, DomainEventDispatcherContext context,
        CancellationToken cancel) {
        return await HandleAsync(@event, context, cancel).ConfigureAwait(false);
    }


    Task<Fin<Unit>> HandleAsync(TDomainEvent @event, DomainEventDispatcherContext context,
        CancellationToken cancel = default);
}