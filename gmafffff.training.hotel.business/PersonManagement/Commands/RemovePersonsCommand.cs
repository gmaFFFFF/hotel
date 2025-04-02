using gmafffff.starterKit.Messaging.Crud;

namespace gmafffff.training.hotel.business.PersonManagement.Commands;

public record RemovePersonsCommand(params Guid[] Ids) : DeleteBusinessCommand<Guid>(Ids);