using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.domain.Dto.PersonManagement;

namespace gmafffff.training.hotel.business.PersonManagement.Commands;

public record UpdatePersonCommand(Guid Id, PersonUpdateDto Changed)
    : UpdateBusinessCommand<Guid, PersonUpdateDto>(Id, Changed);