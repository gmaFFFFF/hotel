using gmafffff.starterKit.Validation;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Validations;
using Validot;

namespace gmafffff.training.hotel.business.PropertyManagement.Validations;

public class AddPersonCommandSpec : LocalizedSpecification,
    ISpecificationHolder<AddPersonCommand> {
    public AddPersonCommandSpec() {
        // Приведение типа к ISpecificationHolder<PersonAddDto> потребовалось, т.к. интерфейс реализован явно
        var personAddDtoSpec = ((ISpecificationHolder<PersonAddDto>)new PersonSpec()).Specification;
        Specification = s => s.Member(memberSelector: m => m.New, personAddDtoSpec);
    }

    public Specification<AddPersonCommand> Specification { get; }
}