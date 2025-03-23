namespace gmafffff.training.hotel.business.PropertyManagement.Commands;

public record RemoveRoomsCommand(params int[] Ids) : BusinessCommand;