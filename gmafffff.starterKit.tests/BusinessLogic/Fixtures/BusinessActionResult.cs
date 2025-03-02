using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public record BusinessActionResult(BusinessActionCommand Command) : BusinessEvent(Command);