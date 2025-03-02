using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.Mappers;

/// <summary>
///     Интерфейс преобразователей сущностей в обменный формат DTO
/// </summary>
/// <typeparam name="TMainEntity">Сущность</typeparam>
/// <typeparam name="TId">Тип идентификатора сущности</typeparam>
/// <typeparam name="TDto">Обменный формат сущности</typeparam>
public interface IEntityMapperForward<TMainEntity, TId, TDto> :
    IEntityMapper<TMainEntity, TId, TDto>
    where TMainEntity : Entity<TId>
    where TId : struct, IEquatable<TId> {
    /// <summary>
    ///     Создаёт новый снимок <paramref name="entity" /> типа <typeparamref name="TDto" />
    /// </summary>
    /// <param name="entity">Сущность, с которой делается снимок</param>
    /// <returns>Снимок с сущности <paramref name="entity" /></returns>
    TDto? Map(TMainEntity? entity);

    /// <summary>
    ///     Изменяет соответствующие свойства/поля <paramref name="targetDto" /> на основе снимка
    ///     <paramref name="sourceEntity" />,
    ///     если <paramref name="targetDto" /> == null, то создается новая сущность
    /// </summary>
    /// <param name="sourceEntity">Сущность</param>
    /// <param name="targetDto">Изменяемый снимок</param>
    /// <returns>Обновлённый снимок с сущности</returns>
    TDto? Update(TMainEntity? sourceEntity, TDto? targetDto);
}