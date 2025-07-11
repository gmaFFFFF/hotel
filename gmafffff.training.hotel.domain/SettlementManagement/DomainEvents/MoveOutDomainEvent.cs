using gmafffff.starterKit.Domain.Events;

namespace gmafffff.training.hotel.domain.SettlementManagement.DomainEvents;

/// <summary>
///     Из номера выселили гостей
/// </summary>
/// <param name="Sender">Освободившийся номер</param>
/// <param name="Visit">Информация о завершившемся визите</param>
public record MoveOutDomainEvent(Room<Guid> Sender, RoomVisit<Guid> Visit) : DomainEvent<Room<Guid>>(Sender);