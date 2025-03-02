namespace gmafffff.starterKit.Messaging;

/// <summary>
///     Запрос фрагмента состояния приложения
/// </summary>
/// <param name="MessageId">Идентификатор запроса</param>
public abstract record Query((uint pageNum, uint pageSize)? Pager = null, Guid MessageId = default)
    : Message(MessageId);