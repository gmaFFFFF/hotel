using gmafffff.starterKit.Domain.Events;

namespace gmafffff.training.hotel.domain.DomainEvents;

public record MoveOutDomainEvent(Room<Guid> Sender, RoomVisit<Guid> Visit) : DomainEvent<Room<Guid>>(Sender);