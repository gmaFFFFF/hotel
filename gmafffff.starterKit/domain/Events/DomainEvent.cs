using System.ComponentModel;

namespace gmafffff.starterKit.Domain.Events;

/// <summary>
///     Событие предметной области. Маркерный интерфейс, предназначенный для использования внутри библиотеки
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IDomainEvent {
    IEntity Sender { get; }
}

/// <summary>
///     Событие предметной области
/// </summary>
/// <param name="Sender">Сущность — отправитель события</param>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public abstract record DomainEvent<TEntity>(TEntity Sender) : IDomainEvent
    where TEntity : IEntity {
    IEntity IDomainEvent.Sender => Sender;
}