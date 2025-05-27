using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public record MyTrigger(int Number, BusinessCommand Command) : TriggerEvent(Command);