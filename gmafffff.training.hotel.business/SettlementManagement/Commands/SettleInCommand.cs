namespace gmafffff.training.hotel.business.SettlementManagement.Commands;

public record SettleInCommand(int RoomId, IEnumerable<Guid> Visitors, DateOnly? DepartureDatePlanned = null)
    : BusinessCommand;