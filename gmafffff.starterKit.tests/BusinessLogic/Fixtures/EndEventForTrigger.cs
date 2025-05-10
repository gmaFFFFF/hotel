using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.Tests.BusinessLogic.Fixtures;

public record EndEventForTrigger(int Number, BusinessCommand Command) : BusinessEvent(Command);