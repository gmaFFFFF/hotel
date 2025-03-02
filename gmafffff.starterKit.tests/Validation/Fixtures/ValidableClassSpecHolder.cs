using gmafffff.starterKit.Validation;
using Validot;

namespace gmafffff.starterKit.tests.Validation.Fixtures;

public class ValidableClassSpecHolder : LocalizedSpecification,
    ISpecificationHolder<ValidableClass> {
    public ValidableClassSpecHolder() {
        Specification<short> ageSpec = s => s
            .GreaterThanOrEqualTo(min: 18)
            .WithErrorCode(ValidationErrorCode.AgeLessThan18);

        Specification<string> nameSpec = s => s
            .NotEmpty()
            .NotWhiteSpace()
            .WithErrorCode(ValidationErrorCode.NameEmpty);

        Specification = s => s
            .Member(memberSelector: m => m.Age, ageSpec)
            .Member(memberSelector: m => m.Name, nameSpec);
    }

    public Specification<ValidableClass> Specification { get; set; }
}