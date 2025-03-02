using gmafffff.starterKit.Validation;
using Validot;

namespace gmafffff.starterKit.tests.Validation.Fixtures;

public class ValidableClass2SpecHolder : LocalizedSpecification,
    ISpecificationHolder<ValidableClass2Ver1>,
    ISpecificationHolder<ValidableClass2Ver2> {
    private readonly Specification<ValidableClass2Ver1> _validableClass2Ver1Spec;
    private readonly Specification<ValidableClass2Ver2> _validableClass2Ver2Spec;

    public ValidableClass2SpecHolder() {
        Specification<string> numberSpec = s => s
            .NotEmpty()
            .NotWhiteSpace()
            .WithErrorCode(ValidationErrorCode.ValidationRoomNumberNo);

        Specification<byte> capacitySpec = s => s
            .GreaterThanOrEqualTo(min: 1)
            .WithErrorCode(ValidationErrorCode.ValidationRoomCapacityNo);

        _validableClass2Ver1Spec = s => s
            .Member(memberSelector: m => m.Number, numberSpec)
            .Member(memberSelector: m => m.Capacity, capacitySpec);
        _validableClass2Ver2Spec = s => s
            .Member(memberSelector: m => m.Number, numberSpec)
            .Member(memberSelector: m => m.Capacity, capacitySpec);
    }

    Specification<ValidableClass2Ver1> ISpecificationHolder<ValidableClass2Ver1>.Specification
        => _validableClass2Ver1Spec;

    Specification<ValidableClass2Ver2> ISpecificationHolder<ValidableClass2Ver2>.Specification
        => _validableClass2Ver2Spec;
}