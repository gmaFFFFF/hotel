namespace gmafffff.starterKit.Messaging.Crud;

public record CreatedBusinessEvent<TEntityId>(TEntityId EntityId, Guid CommandId, Guid MessageId = default)
    : BusinessEvent(CommandId, MessageId)
    where TEntityId : struct, IEquatable<TEntityId> {
    public CreatedBusinessEvent(TEntityId entityId, BusinessCommand command, Guid messageId = default) : this(entityId,
        command.MessageId, messageId) { }
}