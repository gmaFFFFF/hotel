using gmafffff.starterKit.Validation;

namespace gmafffff.starterKit.tests.Validation.Fixtures;

public record ValidableClass2Ver1(int RoomId, string Number, byte Capacity)
    : IValidatableObjectHelperValidot;