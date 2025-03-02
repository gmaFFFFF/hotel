using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public record TestEvent(BusinessCommand Command, Guid MessageId = default): BusinessEvent(Command, MessageId);