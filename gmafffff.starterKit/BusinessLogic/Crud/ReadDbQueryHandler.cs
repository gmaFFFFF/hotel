using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging.Crud;

namespace gmafffff.starterKit.BusinessLogic.Crud;

/// <summary>
///     Специализированная версия <see cref="QueryDbHandler{TQuery, TResult}" />
///     для извлечения сущностей из БД без их отслеживания в <see cref="System.Data.Entity.DBContext" />.
/// </summary>
/// <typeparam name="TQuery">Запрос</typeparam>
/// <typeparam name="TDto">Тип возвращаемого объекта</typeparam>
/// <typeparam name="TEntity">Сущность, содержащаяся в базе данных, содержимое которого извлекается</typeparam>
/// <typeparam name="TEntityId">Тип идентификатора сущности</typeparam>
/// <remarks>
///     К сожалению в DI контейнере Microsoft нельзя осуществить сложную регистрацию открытых обобщённых типов.
///     Поэтому для каждого DTO нужно создать отдельный обработчик.
/// </remarks>
public class ReadDbQueryHandler<TQuery, TEntity, TEntityId, TDto>(
    IRepository<TEntity, TEntityId> repository,
    IEntityMapperForwardExpression<TEntity, TEntityId, TDto> mapper)
    : QueryDbHandler<TQuery, TDto>
    where TQuery : ReadDbQuery<TDto>
    where TDto : class
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId> {
    protected override async Task<IList<TDto>> RunDbQueryAsync(TQuery query, CancellationToken cancel = default) {
        return await repository.GetAsync(query.Filter, mapper.EntityToDto, query.SortOrder, query.Pager, cancel)
            .ConfigureAwait(false);
    }
}