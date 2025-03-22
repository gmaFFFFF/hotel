using LanguageExt;
using Query = gmafffff.starterKit.Messaging.Query;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Интерфейс запроса данных, не предполагающий внесение изменения в состояние пользовательских данных
/// </summary>
/// <typeparam name="TQuery">Запрос типа <see cref="Query" /></typeparam>
/// <typeparam name="TResult">Возвращаемый тип результат</typeparam>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : Query {
    /// <summary>
    ///     Вернуть запрошенные данные
    /// </summary>
    /// <param name="query">Запрос</param>
    /// <param name="cancel">Токен отмены</param>
    /// <returns></returns>
    Task<Fin<IList<TResult>>> RunQueryAsync(TQuery query, CancellationToken cancel = default);
}