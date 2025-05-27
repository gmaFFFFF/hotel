using Microsoft.EntityFrameworkCore;

namespace gmafffff.starterKit.Domain.Events;

/// <summary>
///     Приёмник событий предметной области
/// </summary>
public interface IDomainEventSink {
    /// <summary>
    ///     Поступившие события
    /// </summary>
    IReadOnlyList<IDomainEvent> Events { get; }

    /// <summary>
    ///     Зарегистрировать контекст базы данных для перехвата событий
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    IDomainEventSink RegisterDbContext(DbContext context);

    /// <summary>
    ///     Принять события
    /// </summary>
    /// <param name="event">Испускаемое событие</param>
    IDomainEventSink AddEvent(IDomainEvent @event);
}