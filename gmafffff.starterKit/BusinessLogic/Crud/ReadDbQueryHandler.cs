using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging.Crud;

namespace gmafffff.starterKit.BusinessLogic.Crud;

public class ReadDbQueryHandler<TQuery, TDto, TEntity, TEntityId>(
    IRepository<TEntity, TEntityId> repository,
    IEntityMapperForwardExpression<TEntity, TEntityId, TDto> mapper)
    : QueryDbHandler<TQuery, TDto>
    where TQuery : ReadDbQuery<TDto>
    where TDto : class
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId> {
    public override async Task<IList<TDto>> CreateDbQuery(TQuery query, CancellationToken cancel = default) {
        var filter = query.Filter;
        return await repository.GetAsync(query.Filter, mapper.EntityToDto, query.SortOrder, query.Pager, cancel)
            .ConfigureAwait(false);
    }
}