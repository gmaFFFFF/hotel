namespace gmafffff.starterKit.Messaging;

/// <summary>
///     Запрос фрагмента состояния приложения
/// </summary>
/// <typeparam name="T">Тип, ожидаемый в результате выполнения запроса</typeparam>
/// <param name="SortOrder">Настройка порядка сортировки возвращаемых записей</param>
/// <param name="Pager">Страничная разбивка результата</param>
/// <param name="MessageId">Идентификатор запроса</param>
public abstract record Query<T>(
    Func<IQueryable<T>, IOrderedQueryable<T>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null,
    Guid MessageId = default)
    : Message(MessageId);