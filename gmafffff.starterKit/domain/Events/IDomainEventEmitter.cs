using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace gmafffff.starterKit.Domain.Events;

/// <summary>
///     Сущность, способная создавать события предметной области
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IDomainEventEmitter {
    /// <summary>
    ///     Приемник событий предметной области
    /// </summary>
    [NotMapped]
    IDomainEventSink? DomainEventSink { get; protected set; }

    /// <summary>
    ///     Установить приемник событий предметной области
    /// </summary>
    IDomainEventEmitter SetDomainEventSink(IDomainEventSink domainEventSink) {
        DomainEventSink = domainEventSink;
        return this;
    }

    /// <summary>
    ///     Отключить приемник событий предметной области
    /// </summary>
    IDomainEventEmitter ResetDomainEventSink() {
        DomainEventSink = null;
        return this;
    }
}

/// <summary>
///     Сущность, способная создавать события предметной области
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public interface IDomainEventEmitter<TEntity> : IDomainEventEmitter
    where TEntity : IEntity {
    /// <summary>
    ///     Испустить событие предметной области
    /// </summary>
    /// <param name="event"></param>
    IDomainEventEmitter<TEntity> EmitDomainEvent(DomainEvent<TEntity> @event) {
        DomainEventSink?.AddEvent(@event);
        return this;
    }
}