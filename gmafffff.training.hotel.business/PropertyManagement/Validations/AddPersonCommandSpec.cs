using gmafffff.starterKit.Validation;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Validation;
using Validot;

namespace gmafffff.training.hotel.business.PropertyManagement.Validations;

public class AddPersonCommandSpec : LocalizedSpecification,
    ISpecificationHolder<AddPersonCommand> {
    public AddPersonCommandSpec() {
        // Приведение типа к ISpecificationHolder<PersonAddDto> потребовалось, т.к. интерфейс реализован явно
        var personAddDtoSpec = ((ISpecificationHolder<PersonAddDto>)new PersonSpec()).Specification;
        Specification = s => s.Member(memberSelector: m => m.Person, personAddDtoSpec);
    }

    public Specification<AddPersonCommand> Specification { get; }
}