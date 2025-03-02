using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.Mappers;

/// <summary>
///     Интерфейс преобразователей DTO в сущность
/// </summary>
/// <typeparam name="TMainEntity">Сущность</typeparam>
/// <typeparam name="TId">Тип идентификатора сущности</typeparam>
/// <typeparam name="TDto">Обменный формат сущности</typeparam>
public interface IEntityMapperBackward<TMainEntity, TId, TDto> :
    IEntityMapper<TMainEntity, TId, TDto>
    where TMainEntity : Entity<TId>
    where TId : struct, IEquatable<TId> {
    /// <summary>
    ///     Создаёт новую сущность типа <typeparamref name="TMainEntity" /> на основе её снимка <paramref name="dto" />
    /// </summary>
    /// <param name="dto">Снимок сущности</param>
    /// <returns>Новая сущность</returns>
    TMainEntity? Map(TDto? dto);

    /// <summary>
    ///     Изменяет соответствующие свойства/поля <paramref name="targetEntity" /> на основе снимка
    ///     <paramref name="sourceDto" />,
    ///     если <paramref name="targetEntity" /> == null, то создается новая сущность
    /// </summary>
    /// <param name="sourceDto">Снимок изменённой сущности</param>
    /// <param name="targetEntity">Изменяемая сущность</param>
    /// <returns>Изменённая сущность <paramref name="targetEntity" /></returns>
    TMainEntity? Update(TDto? sourceDto, TMainEntity? targetEntity);
}