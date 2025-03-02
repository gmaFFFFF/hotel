using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public record BusinessActionCommand(bool IsCorrect) : BusinessCommand;