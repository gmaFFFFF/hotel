using gmafffff.training.hotel.domain.Dto.PersonManagement;

namespace gmafffff.training.hotel.business.PersonManagement.Commands;

public record AddPersonCommand(PersonAddDto Person) : BusinessCommand;