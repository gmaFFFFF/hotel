using System.Reflection;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Validot;

namespace gmafffff.starterKit.Di;

public static class RegisterServicesExtensions {
    /// <summary>
    ///     Служебные интерфейсы, которые не требуется регистрировать
    /// </summary>
    private static readonly Type[] IgnoreInterfaces = [
        typeof(IEntityMapper<,,>),
        typeof(IEntityMapperForward<,,>),
        typeof(IEntityMapperBackward<,,>),
        typeof(IEntityMapperDuplex<,,>),
        typeof(IEntityMapperForwardExpression<,,>),
        typeof(IRepository<,>),
        typeof(IRepositoryFactory<,,>),
        typeof(IDomainEventHandler),
        typeof(IDisposable),
        typeof(IAsyncDisposable)
    ];

    #region Данные

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей оперативные склады,
    ///     реализующие интерфейс <see cref="IRepository{T,TId}" />,
    ///     а также фабрики, реализующие интерфейс <see cref="IRepositoryFactory{TRepo, TEntity, TId}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection @this, params Assembly[] assemblies) {
        return @this
            .Scan(scan => {
                var selector = assemblies.Length == 0
                    ? scan.FromApplicationDependencies()
                    : scan.FromAssemblies(assemblies);

                selector
                    .AddClasses(@class => @class.AssignableTo(typeof(IRepository<,>)))
                    .AsImplementedInterfaces(predicate: @interface =>
                        @interface.IsGenericType
                            ? !IgnoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                            : !IgnoreInterfaces.Contains(@interface))
                    .WithScopedLifetime();
            })
            .Scan(scan => {
                var selector = assemblies.Length == 0
                    ? scan.FromApplicationDependencies()
                    : scan.FromAssemblies(assemblies);
                selector
                    .AddClasses(@class => @class.AssignableTo(typeof(IRepositoryFactory<,,>)))
                    .AsImplementedInterfaces(predicate: @interface =>
                        @interface.IsGenericType
                            ? !IgnoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                            : !IgnoreInterfaces.Contains(@interface))
                    .WithSingletonLifetime();
            });
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей преобразователи,
    ///     реализующие интерфейс <see cref="IEntityMapper{TMainEntity,TId,TDto}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddEntityMappers(this IServiceCollection @this, params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IEntityMapper<,,>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsSelfWithInterfaces(predicate: @interface =>
                    @interface.IsGenericType
                        ? !IgnoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                        : !IgnoreInterfaces.Contains(@interface))
                .WithSingletonLifetime();
        });
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей преобразователи,
    ///     реализующие интерфейс <see cref="IEntityMapper{TMainEntity,TId,TDto}" />,
    ///     а также находит конфигурации mapster и регистрирует их в глобальной конфигурации
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <remarks>
    ///     Комбинация методов <see cref="AddEntityMappers(IServiceCollection, Assembly[])" />
    ///     и <see cref="RegisterMapsterConfigs(Assembly[])" />
    /// </remarks>
    public static IServiceCollection AddEntityMappersWithConfig(this IServiceCollection @this,
        params Assembly[] assemblies) {
        RegisterMapsterConfigs(assemblies);
        return @this.AddEntityMappers(assemblies);
    }

    /// <summary>
    ///     Находит конфигурации mapster и регистрирует их в глобальной конфигурации
    ///     <see cref="TypeAdapterConfig.GlobalSettings" />
    /// </summary>
    public static void RegisterMapsterConfigs(params Assembly[] assemblies) {
        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : AppDomain.CurrentDomain.GetAssemblies();

        TypeAdapterConfig.GlobalSettings.Scan(assembliesToScan);
    }    

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей обработчики событий домена <see cref="DomainEvent{TEntity}" />,
    ///     реализующие интерфейс <see cref="IDomainEventHandler{TDomainEvent}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces(predicate: @interface =>
                    @interface.IsGenericType
                        ? !IgnoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                        : !IgnoreInterfaces.Contains(@interface))
                .WithTransientLifetime();
        });
    }

    #endregion

    #region Бизнес-логика

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей исполнитель команд <see cref="IBusinessActionRunner{TCommand}" />
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static IServiceCollection AddBusinessActionRunner(this IServiceCollection @this) {
        return @this.AddTransient(typeof(IBusinessActionRunner<>), typeof(BusinessActionRunner<>));
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей проверяющих бизнес-ограничения,
    ///     реализующих интерфейс <see cref="IBusinessConstraintCheck{TCommand}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddBusinessConstraintsChecks(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IBusinessConstraintCheck<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей обработчики бизнес команд,
    ///     реализующие класс <see cref="BusinessCommandDbHandler{TCommand,TEntity,TId,TRepo,TLoad,TResult}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddBusinessCommandDbHandlers(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(BusinessCommandDbHandler<,,,,,>)))
                .AsImplementedInterfaces(predicate: @interface =>
                    @interface.IsGenericType
                        ? !IgnoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                        : !IgnoreInterfaces.Contains(@interface))
                .WithTransientLifetime();
        });
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей обработчики запросов,
    ///     реализующие интерфейс <see cref="IQueryHandler{TQuery,TResult}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddQueryHandlers(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces(predicate: @interface =>
                    @interface.IsGenericType
                        ? !IgnoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                        : !IgnoreInterfaces.Contains(@interface))
                .WithTransientLifetime();
        });
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей конверторы <see cref="TriggerEvent" />,
    ///     реализующие интерфейс <see cref="ITriggerEventToCommandTranslator{T}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddTriggerEventToCommandTranslators(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(ITriggerEventToCommandTranslator<>)))
                .AsImplementedInterfaces(predicate: @interface =>
                    @interface.IsGenericType
                        ? !IgnoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                        : !IgnoreInterfaces.Contains(@interface))
                .WithTransientLifetime();
        });
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей проверяющих,
    ///     реализующих интерфейс <see cref="ISpecificationHolder{T}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <exception cref="ValidotException">
    ///     Если найдено несколько проверяющих <see cref="IValidator{T}" /> одного и того же
    ///     типа
    /// </exception>
    public static IServiceCollection AddValidotValidators(this IServiceCollection @this, params Assembly[] assemblies) {
        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : AppDomain.CurrentDomain.GetAssemblies();

        var holders = Validator.Factory.FetchHolders(assembliesToScan)
            .GroupBy(h => h.SpecifiedType)
            .Select(v => new {
                v.First().ValidatorType,
                ValidatorInstance = v.First().CreateValidator(),
                ValidatorsCount = v.Count()
            }).ToArray();

        // Если для одного типа встретилось несколько проверяющих, то лучше об этом громко заявить
        var errors = holders.Where(h => h.ValidatorsCount > 1)
            .Select(s => (s.ValidatorType, s.ValidatorsCount))
            .ToArray();
        if (errors.Any()) {
            var exception =
                new ValidotException(
                    "Встретилось несколько проверяющих типа IValidator<T>. Количество проверяющих каждого типа занесено в свойство «Data»");
            foreach (var (key, val) in errors)
                exception.Data.Add(key, val);
            throw exception;
        }

        // Регистрация
        foreach (var holder in holders)
            @this.AddSingleton(holder.ValidatorType, holder.ValidatorInstance);

        return @this;
    }

    #endregion
}