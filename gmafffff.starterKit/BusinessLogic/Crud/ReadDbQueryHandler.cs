using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.Messaging.Crud;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace gmafffff.starterKit.BusinessLogic.Crud;

/// <summary>
///     Специализированная версия <see cref="QueryDbHandler{TQuery, TResult}" />
///     для извлечения сущностей из БД без их отслеживания в <see cref="Entity{TId}.DBContext" />.
/// </summary>
/// <typeparam name="TQuery">Запрос</typeparam>
/// <typeparam name="TEntity">Сущность, содержащаяся в базе данных, содержимое которого извлекается</typeparam>
/// <typeparam name="TEntityId">Тип идентификатора сущности</typeparam>
/// <typeparam name="TRepo">Тип оперативного склада, используемого для выполнения запросов к БД</typeparam>
/// <typeparam name="TDto">Спроецированный тип возвращаемого объекта</typeparam>
/// <remarks>
///     К сожалению в DI контейнере Microsoft нельзя осуществить сложную регистрацию открытых обобщённых типов.
///     Поэтому для каждого DTO нужно создать отдельный обработчик
///     или воспользоваться <see cref="QueryHandlerFactory" />.
/// </remarks>
public partial class ReadDbQueryHandler<TQuery, TEntity, TEntityId, TRepo, TDto>(
    TRepo repository,
    IEntityMapperForwardExpression<TEntity, TEntityId, TDto> mapper,
    ILogger<IQueryHandler<TQuery, TDto>>? logger = null)
    : QueryDbHandler<TQuery, TDto>
    where TQuery : Query<TDto>
    where TEntityId : struct, IEquatable<TEntityId>
    where TEntity : Entity<TEntityId>
    where TRepo : IRepositoryReadOnly<TEntity, TEntityId>
    where TDto : class {
    /// <summary>
    ///     CRC16 для <see cref="gmafffff.starterKit.BusinessLogic.Crud.ReadDbQueryHandler{TQuery,TEntity,TEntityId,TDto}" />
    /// </summary>
    public const int EventIdBase = 0x566e;

    /// <summary>
    ///     Журнал
    /// </summary>
    private readonly ILogger<IQueryHandler<TQuery, TDto>> _logger =
        logger ?? NullLogger<IQueryHandler<TQuery, TDto>>.Instance;

    protected override async Task<IList<TDto>> RunDbQueryAsync(TQuery query, CancellationToken cancel = default) {
        const string argumentOutOfRangeMessage = "Непредусмотренный тип запроса на чтение из БД";

        var result =
            query switch {
                ReadDbQuery<TDto> queryByDto => await repository.GetAsync(queryByDto.Filter,
                    mapper.EntityToDto, queryByDto.SortOrder, queryByDto.Pager, cancel).ConfigureAwait(false),
                ReadDbQuery<TEntity, TDto> queryByEntity => await repository.GetAsync(queryByEntity.Filter,
                    mapper.EntityToDto, queryByEntity.SortOrder, queryByEntity.Pager, cancel).ConfigureAwait(false),
                _ => throw new ArgumentOutOfRangeException(nameof(query), query, argumentOutOfRangeMessage)
            };

        ExecuteQueryLog(query);
        return result;
    }

    [LoggerMessage(EventId = EventIdBase + 1, Level = LogLevel.Trace, Message = "Выполнен запрос к БД: {@Query}",
        EventName = "ExecuteQuery")]
    private partial void ExecuteQueryLog(TQuery query);
}