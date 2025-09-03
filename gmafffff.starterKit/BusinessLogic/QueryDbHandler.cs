using gmafffff.starterKit.AppError;
using gmafffff.starterKit.Messaging;
using LanguageExt;
using Light.GuardClauses;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Каркас реализации <see cref="IQueryHandler{TQuery, TResult}" /> для запросов к БД.
///     Если нужно извлечь сущность, которая имеет свой оперативный склад, то рекомендуется использовать
///     <see cref="Crud.ReadDbQueryHandler{TQuery, TEntity, TEntityId, TRepo, TDto}"/>.
/// </summary>
/// <typeparam name="TQuery">Запрос типа <see cref="Query" /></typeparam>
/// <typeparam name="TResult">Возвращаемый тип результат</typeparam>
/// <remarks>
///     Необходимо переопределить метод <see cref="QueryDbHandler{TQuery, TResult}.RunDbQueryAsync(TQuery, CancellationToken)"/>
///     так, чтобы он запустил на выполнение необходимый запрос к БД
/// </remarks>
public abstract class QueryDbHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : Query<TResult> {
    public async Task<Fin<IList<TResult>>> RunQueryAsync(TQuery query, CancellationToken cancel = default) {
        query.MustNotBeNull();
        try {
            var result = await RunDbQueryAsync(query, cancel).ConfigureAwait(false);
            return Fin<IList<TResult>>.Succ(result.ToList());
        }
        catch (OperationCanceledException) {
            return AppErrorHelper.NewError(AppErrorCode.OperationCancel);
        }
    }

    protected abstract Task<IList<TResult>> RunDbQueryAsync(TQuery query, CancellationToken cancel = default);
}