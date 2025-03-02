namespace gmafffff.starterKit.Messaging;

/// <summary>
///     Сообщение об изменениях, произошедших в системе
/// </summary>
/// <param name="CommandId">Идентификатор <see cref="BusinessCommand" />, вызвавшего событие</param>
public abstract record BusinessEvent(Guid CommandId, Guid MessageId = default) : Message(MessageId) {
    public BusinessEvent(BusinessCommand command, Guid messageId = default) : this(command.MessageId, messageId) { }
}