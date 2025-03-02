using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public record DbHandlerCommand(bool IsSuccessLoad = true, bool IsSaveResult = true, Exception? Exception = null)
    : BusinessCommand;