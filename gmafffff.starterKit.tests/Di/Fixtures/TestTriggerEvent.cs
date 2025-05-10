using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public record TestTriggerEvent(BusinessCommand Command) : TriggerEvent(Command);