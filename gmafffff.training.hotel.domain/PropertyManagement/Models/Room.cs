using gmafffff.starterKit.Domain.Events;
using gmafffff.training.hotel.domain.SettlementManagement.DomainEvents;

namespace gmafffff.training.hotel.domain.PropertyManagement.Models;

/// <summary>
///     Номер отеля
/// </summary>
public partial class Room<TPersonId> : Entity<int>,
    IDomainEventEmitter<Room<Guid>>
    where TPersonId : struct, IEquatable<TPersonId> {
    /// <summary>
    ///     Посещение номера
    /// </summary>
    private RoomVisit<TPersonId>? _visit;

    /// <summary>
    ///     Описание
    /// </summary>
    public RoomDetails RoomDetails { get; set; }

    /// <summary>
    ///     Посещение номера
    /// </summary>
    public RoomVisit<TPersonId>? Visit {
        get => _visit;
        set {
            if (_visit is not null && value is null) {
                // Так как изначально не конкретизировал параметр TPersonId класса Room<TPersonId>, то …
                var sender = (object)this as Room<Guid>;
                var visit = (object)Visit as RoomVisit<Guid>;

                ((IDomainEventEmitter<Room<Guid>>)this).EmitDomainEvent(new MoveOutDomainEvent(sender, visit));
            }

            _visit = value;
        }
    }

    /// <summary>
    ///     Уборки номера
    /// </summary>
    public ICollection<RoomCleaning> RoomCleanings { get; set; } = null!;

    /// <summary>
    ///     Свободен ли номер
    /// </summary>
    public bool IsFree => Visit is null;

    /// <summary>
    ///     Убран ли номер
    /// </summary>
    public bool? IsClean => !RoomCleanings?.Any(clean => !clean.IsClean);

    /// <summary>
    ///     Сегодня планируется ли выселение
    /// </summary>
    public bool? IsFreeToday
        => Visit?.DepartureDatePlanned?.DayNumber <= DateOnly.FromDateTime(DateTime.Today).DayNumber;

    IDomainEventSink? IDomainEventEmitter.DomainEventSink { get; set; }
}