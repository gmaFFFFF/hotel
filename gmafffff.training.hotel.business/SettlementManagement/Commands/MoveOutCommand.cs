namespace gmafffff.training.hotel.business.SettlementManagement.Commands;

public record MoveOutCommand(int RoomId, DateOnly? DepartureDate = null) : BusinessCommand;