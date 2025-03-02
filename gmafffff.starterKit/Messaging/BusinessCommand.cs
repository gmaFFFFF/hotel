namespace gmafffff.starterKit.Messaging;

/// <summary>
///     Сообщение для внесения изменения в состояние приложения
/// </summary>
/// <param name="MessageId">Уникальный идентификатор сообщения-команды</param>
public abstract record BusinessCommand(Guid MessageId = default) : Message(MessageId);