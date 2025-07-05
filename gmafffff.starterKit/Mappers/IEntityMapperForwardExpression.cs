using System.Linq.Expressions;
using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.Mappers;

/// <summary>
///     Базовый интерфейс преобразователей сущностей в обменный формат DTO и обратно
/// </summary>
/// <typeparam name="TMainEntity">Сущность</typeparam>
/// <typeparam name="TId">Тип идентификатора сущности</typeparam>
/// <typeparam name="TDto">Обменный формат сущности</typeparam>
public interface IEntityMapperForwardExpression<TMainEntity, TId, TDto> :
    IEntityMapper<TMainEntity, TId, TDto>
    where TMainEntity : Entity<TId>
    where TId : struct, IEquatable<TId> {
    /// <summary>
    ///     Для использования в качестве аргумента <see cref="IQueryable{TMainEntity}.Select" />
    /// </summary>
    Expression<Func<TMainEntity, TDto>> EntityToDto { get; }
}