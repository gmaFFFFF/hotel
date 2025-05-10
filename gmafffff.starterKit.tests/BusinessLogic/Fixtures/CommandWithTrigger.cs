using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.Tests.BusinessLogic.Fixtures;

public record CommandWithTrigger(int number, bool Error = false) : BusinessCommand;