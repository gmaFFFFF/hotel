using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public record EndEventForTrigger(int Number, BusinessCommand Command) : BusinessEvent(Command);