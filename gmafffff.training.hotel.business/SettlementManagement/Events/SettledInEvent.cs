using gmafffff.training.hotel.business.SettlementManagement.Commands;

namespace gmafffff.training.hotel.business.SettlementManagement.Events;

public record SettledInEvent(int RoomId, SettleInCommand Command) : BusinessEvent(Command);