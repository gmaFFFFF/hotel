using System.Collections.Immutable;
using gmafffff.starterKit.AppError;
using LanguageExt;
using Query = gmafffff.starterKit.Messaging.Query;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Реализация по умолчанию <see cref="IQueryHandler{TQuery, TResult}" /> для запросов к БД
/// </summary>
/// <typeparam name="TQuery">Запрос типа <see cref="Query" /></typeparam>
/// <typeparam name="TResult">Возвращаемый тип результат</typeparam>
public abstract class QueryDbHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : Query {
    public async Task<Fin<IList<TResult>>> RunQueryAsync(TQuery query, CancellationToken cancel = default) {
        try {
            var result = await CreateDbQuery(query, cancel).ConfigureAwait(false);
            return Fin<IList<TResult>>.Succ(result.ToList());
        }
        catch (OperationCanceledException) {
            throw;
        }
        catch (Exception ex) {
            return AppErrorHelper.NewError(ex);
        }
    }

    public abstract Task<IImmutableList<TResult>> CreateDbQuery(TQuery query, CancellationToken cancel = default);
}