using System.Linq.Expressions;
using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.Messaging.Crud;

/// <summary>
///     Запрос в базу данных
/// </summary>
/// <typeparam name="TDto">Тип, в который необходимо спроецировать результат запроса</typeparam>
/// <param name="Filter">Спецификация извлекаемых данных</param>
/// <param name="SortOrder">Настройка порядка сортировки возвращаемых записей</param>
/// <param name="Pager">Страничная разбивка результата</param>
/// <param name="MessageId">Идентификатор запроса</param>
public abstract record ReadDbQuery<TDto>(
    Expression<Func<TDto, bool>> Filter,
    Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null,
    Guid MessageId = default) : Query<TDto>(SortOrder, Pager, MessageId);

/// <summary>
///     Запрос в базу данных с возможностью фильтрации непосредственно по сущности <see cref="Entity{TId}"/>,
///     содержащейся в БД
/// </summary>
/// <typeparam name="TEntity">Тип сущности БД <see cref="Entity{TId}"/>, из которого извлекаются данные</typeparam>
/// <typeparam name="TDto">Тип, в который необходимо спроецировать результат запроса</typeparam>
/// <param name="Filter">Спецификация извлекаемых данных</param>
/// <param name="SortOrder">Настройка порядка сортировки возвращаемых записей</param>
/// <param name="Pager">Страничная разбивка результата</param>
/// <param name="MessageId">Идентификатор запроса</param>
public abstract record ReadDbQuery<TEntity, TDto>(
    Expression<Func<TEntity, bool>> Filter,
    Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null,
    Guid MessageId = default) : Query<TDto>(SortOrder, Pager, MessageId)
    where TEntity : IEntity;