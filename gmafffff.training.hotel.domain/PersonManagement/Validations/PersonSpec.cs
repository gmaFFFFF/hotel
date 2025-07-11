using gmafffff.starterKit.Validation;
using gmafffff.training.hotel.domain.Error;
using gmafffff.training.hotel.domain.PersonManagement.Dto;
using Validot;

namespace gmafffff.training.hotel.domain.PersonManagement.Validations;

public class PersonSpec : LocalizedSpecification,
    ISpecificationHolder<PersonFullName>,
    ISpecificationHolder<PersonAddDto>,
    ISpecificationHolder<PersonUpdateDto> {
    private readonly Specification<PersonAddDto> _personAddDtoSpec;

    private readonly Specification<PersonFullName> _personFullNameSpec;
    private readonly Specification<PersonUpdateDto> _personUpdateDtoSpec;

    public PersonSpec() {
        Specification<string> firstName = s => s
            .NotEmpty()
            .WithErrorCode(ErrorCode.ValidationPersonNameNo)
            .NotWhiteSpace()
            .WithErrorCode(ErrorCode.ValidationPersonNameNo);

        Specification<string> surName = s => s
            .NotEmpty()
            .WithErrorCode(ErrorCode.ValidationSurNameNo)
            .NotWhiteSpace()
            .WithErrorCode(ErrorCode.ValidationSurNameNo);

        _personFullNameSpec = s => s
            .Member(memberSelector: m => m.FirstName, firstName)
            .Member(memberSelector: m => m.SurName, surName);

        _personAddDtoSpec = s => s
            .Member(memberSelector: m => m.FirstName, firstName)
            .Member(memberSelector: m => m.SurName, surName);

        _personUpdateDtoSpec = s => s
            .Member(memberSelector: m => m.FirstName, firstName)
            .Member(memberSelector: m => m.SurName, surName);
    }

    Specification<PersonAddDto> ISpecificationHolder<PersonAddDto>.Specification => _personAddDtoSpec;
    Specification<PersonFullName> ISpecificationHolder<PersonFullName>.Specification => _personFullNameSpec;
    Specification<PersonUpdateDto> ISpecificationHolder<PersonUpdateDto>.Specification => _personUpdateDtoSpec;
}