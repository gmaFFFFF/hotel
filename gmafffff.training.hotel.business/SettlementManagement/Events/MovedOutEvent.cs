using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.SettlementManagement.Events;

public record MovedOutEvent(AccommodationReport<Guid> Report, MoveOutCommand Command) : BusinessEvent(Command);