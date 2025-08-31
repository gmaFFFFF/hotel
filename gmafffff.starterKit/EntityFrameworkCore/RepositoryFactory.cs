using gmafffff.starterKit.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.EntityFrameworkCore;

/// <summary>
///     Стандартная реализация интерфейса <see cref="IRepositoryFactory{TRepo,TEntity,TId}" /> для EF Core
/// </summary>
/// <typeparam name="TRepoInterface"></typeparam>
/// <typeparam name="TRepo"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TId"></typeparam>
/// <param name="provider"></param>
internal class RepositoryFactory<TRepoInterface, TRepo, TEntity, TId>(IServiceProvider provider)
    : IRepositoryFactory<TRepoInterface, TEntity, TId>
    where TRepoInterface : IRepositoryReadOnly<TEntity, TId>
    where TRepo : TRepoInterface
    where TEntity : Entity<TId>
    where TId : struct, IEquatable<TId> {
    public TRepoInterface CreateTransient() {
        var dbContextType = typeof(TRepo)
            .GetConstructors()
            .SelectMany(cntr => cntr.GetParameters())
            .First(param => param.ParameterType.IsAssignableTo(typeof(DbContext)))
            .ParameterType;
        var factoryType = typeof(IDbContextFactory<>).MakeGenericType(dbContextType);
        var factory = provider.GetRequiredService(factoryType);
        var createDbContextMethod = factoryType.GetMethod(nameof(IDbContextFactory<DbContext>.CreateDbContext));
        var dbContext = createDbContextMethod.Invoke(factory, parameters: null);


        return dbContext is not null
            ? ActivatorUtilities.CreateInstance<TRepo>(new SafeRootServiceProvider(provider), dbContext)
            : ActivatorUtilities.CreateInstance<TRepo>(new SafeRootServiceProvider(provider));
    }
}

/// <summary>
///     Обертка над <see cref="IServiceProvider" />.
///     Перехватывает исключения, например, <see cref="InvalidOperationException" />:
///     "Cannot resolve scoped service 'X' from root provider ()" и возвращает вместо него null.
///     Для автоматической попытки подбора правильного конструктора этот вариант не создаст проблем
/// </summary>
file record SafeRootServiceProvider(IServiceProvider Provider) : IServiceProvider {
    public object? GetService(Type serviceType) {
        try {
            return Provider.GetService(serviceType);
        }
        catch {
            return null;
        }
    }
}