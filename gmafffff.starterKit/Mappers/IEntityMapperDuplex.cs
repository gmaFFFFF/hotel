using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.Mappers;

/// <summary>
///     Интерфейс преобразователей сущностей в обменный формат DTO и обратно
/// </summary>
/// <typeparam name="TMainEntity">Сущность</typeparam>
/// <typeparam name="TId">Тип идентификатора сущности</typeparam>
/// <typeparam name="TDto">Обменный формат сущности</typeparam>
public interface IEntityMapperDuplex<TMainEntity, TId, TDto> :
    IEntityMapperForward<TMainEntity, TId, TDto>,
    IEntityMapperBackward<TMainEntity, TId, TDto>
    where TMainEntity : Entity<TId>
    where TId : struct, IEquatable<TId>;