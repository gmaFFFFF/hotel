using gmafffff.starterKit.AppError;
using gmafffff.starterKit.Messaging;
using LanguageExt;
using Light.GuardClauses;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Реализация по умолчанию <see cref="IQueryHandler{TQuery, TResult}" /> для запросов к БД
/// </summary>
/// <typeparam name="TQuery">Запрос типа <see cref="Query" /></typeparam>
/// <typeparam name="TResult">Возвращаемый тип результат</typeparam>
public abstract class QueryDbHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : Query<TResult> {
    public async Task<Fin<IList<TResult>>> RunQueryAsync(TQuery query, CancellationToken cancel = default) {
        query.MustNotBeNull();
        try {
            var result = await CreateDbQuery(query, cancel).ConfigureAwait(false);
            return Fin<IList<TResult>>.Succ(result.ToList());
        }
        catch (OperationCanceledException) {
            return AppErrorHelper.NewError(AppErrorCode.OperationCancel);
        }
        catch (Exception ex) {
            return AppErrorHelper.NewError(ex);
        }
    }

    public abstract Task<IList<TResult>> CreateDbQuery(TQuery query, CancellationToken cancel = default);
}