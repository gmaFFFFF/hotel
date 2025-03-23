namespace gmafffff.starterKit.Messaging.Standard;

public record RemovedBusinessEvent<TEntityId>(TEntityId EntityId, Guid CommandId, Guid MessageId = default)
    : BusinessEvent(CommandId, MessageId)
    where TEntityId : struct, IEquatable<TEntityId> {
    public RemovedBusinessEvent(TEntityId entityId, BusinessCommand command, Guid messageId = default) : this(entityId,
        command.MessageId, messageId) { }
}