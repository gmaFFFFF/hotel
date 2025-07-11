using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.domain.PersonManagement.Dto;

namespace gmafffff.training.hotel.business.PersonManagement.Commands;

public record AddPersonCommand(PersonAddDto New) : CreateBusinessCommand<PersonAddDto>(New);