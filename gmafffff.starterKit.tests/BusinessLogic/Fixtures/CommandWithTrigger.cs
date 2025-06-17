namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public record CommandWithTrigger(int Number, bool ThrowException = false, bool Error = false, bool Cancel = false)
    : DbHandlerCommand;