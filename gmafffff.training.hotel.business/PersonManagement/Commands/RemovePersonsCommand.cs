namespace gmafffff.training.hotel.business.PersonManagement.Commands;

public record RemovePersonsCommand(params Guid[] Ids) : BusinessCommand;