namespace gmafffff.starterKit.Messaging;

/// <summary>
///     Запрос фрагмента состояния приложения
/// </summary>
/// <param name="MessageId">Идентификатор запроса</param>
public abstract record Query<T>(
    Func<IQueryable<T>, IOrderedQueryable<T>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null,
    Guid MessageId = default)
    : Message(MessageId);