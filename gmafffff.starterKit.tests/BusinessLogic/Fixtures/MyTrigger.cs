using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.Tests.BusinessLogic.Fixtures;

public record MyTrigger(int Number, BusinessCommand Command) : TriggerEvent(Command);