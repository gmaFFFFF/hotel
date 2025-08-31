namespace gmafffff.starterKit.Domain;

/// <summary>
///     Фабрика для создания оперативных складов со временем жизни «transient»
/// </summary>
/// <typeparam name="TRepoInterface"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TId"></typeparam>
public interface IRepositoryFactory<out TRepoInterface, TEntity, TId>
    where TRepoInterface : IRepositoryReadOnly<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : struct, IEquatable<TId> {
    /// <summary>
    ///     Создает оперативный склад со временем жизни «transient»
    /// </summary>
    TRepoInterface CreateTransient();
}