using LanguageExt;
using Query = gmafffff.starterKit.Messaging.Query;

namespace gmafffff.starterKit.BusinessLogic;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : Query {
    //Вернуть запрошенные данные
    Task<Fin<IList<TResult>>> QueryAsync(TQuery query, CancellationToken cancel = default);
}