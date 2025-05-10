namespace gmafffff.starterKit.Messaging;

/// <summary>
///     Сигнал для вызова связанных команд (событие интеграции)
/// </summary>
public abstract record TriggerEvent(Guid CommandId, Guid MessageId = default)
    : BusinessEvent(CommandId, MessageId) {
    public TriggerEvent(BusinessCommand command, Guid messageId = default) : this(command.MessageId, messageId) { }
}