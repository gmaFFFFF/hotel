using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public record DbHandlerEvent(DbHandlerStatus Result, Guid CommandId, Guid MessageId = default)
    : BusinessEvent(CommandId, MessageId) {
    public DbHandlerEvent(DbHandlerStatus Result, DbHandlerCommand command) : this(Result, command.MessageId) { }
}