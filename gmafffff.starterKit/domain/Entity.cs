using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace gmafffff.starterKit.Domain;

/// <summary>
///     Маркерный интерфейс, предназначенный для использования внутри библиотеки
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IEntity;

/// <summary>
///     Сущность, предназначенная для длительного хранения.
///     Переопределяет семантику сравнения на основе значения идентификатора
/// </summary>
/// <typeparam name="TId"></typeparam>
public abstract class Entity<TId> : IEntity
    where TId : struct, IEquatable<TId> {
    protected Entity(TId id) : this() {
        Id = id;
    }

    protected Entity() {
        _hashCode = new Lazy<int>(() => CalcHashCode(this));
        _hashCodeWithoutKey = new Lazy<int>(() => CalcHashCode(this));
    }

    /// <summary>
    ///     Уникальный идентификатор сущности
    /// </summary>
    public TId Id { get; set; }

    #region Поддержка сравнения

    /// <summary>
    ///     Сущность извлечена из хранилища (<c>true</c>), а не создана приложением
    /// </summary>
    /// <returns>
    ///     True - сущность извлечена из хранилища, false - сущность временная
    /// </returns>
    [Pure]
    public virtual bool IsFromRepository() {
        return Id switch {
            // EF Core устанавливает значение int / long < 0 при добавлении сущности в DbContext
            (long or int) and <= 0 => false,
            Guid guid when guid == Guid.Empty => false,
            _ => !Id.Equals(default)
        };
    }

    /// <summary>
    ///     Кэшированный хэш-код
    /// </summary>
    [NotMapped] private readonly Lazy<int> _hashCode;

    /// <summary>
    ///     Кэшированный хэш-код сущности без ид
    /// </summary>
    [NotMapped] private readonly Lazy<int> _hashCodeWithoutKey;

    /// <summary>
    ///     Рассчитывает хэш-код
    /// </summary>
    /// <param name="entity">Сущность, для которой производятся вычисления </param>
    /// <returns></returns>
    [Pure]
    public static int CalcHashCode<TEntity>(TEntity entity)
        where TEntity : Entity<TId> {
        var hash = new HashCode();
        hash.Add(typeof(TEntity).FullName?.GetHashCode() ?? typeof(TEntity).Name.GetHashCode());
        hash.Add(entity.Id.GetHashCode() ^ 31); // XOR for random distribution
        // (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

        return hash.ToHashCode();
    }

    [Pure]
    public override int GetHashCode() {
        return IsFromRepository() ? _hashCode.Value : _hashCodeWithoutKey.Value;
    }

    [Pure]
    public bool Equals(Entity<TId>? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other.GetType() != GetType()) return false;
        return IsFromRepository() && other.IsFromRepository() && Id.Equals(other.Id);
    }

    [Pure]
    public override bool Equals(object? obj) {
        return Equals(obj as Entity<TId>);
    }

    [Pure]
    public static bool operator ==(Entity<TId>? lhs, Entity<TId>? rhs) {
        return lhs?.Equals(rhs) ?? Equals(rhs, objB: null);
    }

    [Pure]
    public static bool operator !=(Entity<TId>? lhs, Entity<TId>? rhs) {
        return !(lhs == rhs);
    }

    #endregion
}