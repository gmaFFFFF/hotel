using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.Mappers;

/// <summary>
///     Базовый интерфейс преобразователей сущностей в обменный формат DTO и обратно
/// </summary>
/// <typeparam name="TMainEntity">Сущность</typeparam>
/// <typeparam name="TId">Тип идентификатора сущности</typeparam>
/// <typeparam name="TDto">Обменный формат сущности</typeparam>
public interface IEntityMapper<TMainEntity, TId, TDto>
    where TMainEntity : Entity<TId>
    where TId : struct, IEquatable<TId>;