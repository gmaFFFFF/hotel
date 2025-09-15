using System.Reflection;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;
using Validot;

namespace gmafffff.starterKit.Di;

public static class RegisterServicesExtensions {
    /// <summary>
    ///     Служебные интерфейсы, которые не требуется регистрировать
    /// </summary>
    private static readonly Type[] IgnoreInterfaces = [
        typeof(IEntityMapper<,,>),
        typeof(IDomainEventHandler),
        typeof(IDisposable),
        typeof(IAsyncDisposable)
    ];

    private static readonly Type[] IgnoreAttributes = [
        typeof(MapperAttribute)
    ];

    /// <summary>
    ///     Проверяет нужно ли регистрировать интерфейс или он является служебным
    /// </summary>
    /// <param name="interface">Тип проверяемого интерфейса</param>
    /// <returns></returns>
    private static bool IsRegisterInterface(Type @interface) {
        var isIgnored = @interface.IsGenericType
            ? IgnoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
            : IgnoreInterfaces.Contains(@interface);

        var hasIgnoredAttribute = IgnoreAttributes.Any(ignore
            => @interface
                .CustomAttributes
                .Select(attr => attr.AttributeType)
                .Contains(ignore));

        return !isIgnored && !hasIgnoredAttribute;
    }

    #region Валидация

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

    #region Домен

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей оперативные склады,
    ///     реализующие интерфейс <see cref="IRepositoryReadOnly{T,TId}" /> и <see cref="IRepository{T,TId}" />,
    ///     а также фабрики, реализующие интерфейс <see cref="IRepositoryFactory{TRepo, TEntity, TId}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection @this, params Assembly[] assemblies) {
        var serviceCollection = @this
            .Scan(scan => {
                var selector = assemblies.Length == 0
                    ? scan.FromApplicationDependencies()
                    : scan.FromAssemblies(assemblies);

                selector
                    .AddClasses(@class => @class.AssignableTo(typeof(IRepositoryReadOnly<,>)))
                    .AsImplementedInterfaces(predicate: IsRegisterInterface)
                    .WithScopedLifetime();
            });

        var isRepoInterface = (Type @interface) =>
            @interface.IsGenericType &&
            @interface.GetGenericTypeDefinition() == typeof(IRepositoryReadOnly<,>);

        var repoServices = serviceCollection
            .Where(service => service
                .ServiceType
                .GetInterfaces()
                .Any(isRepoInterface))
            .ToArray();
        foreach (var service in repoServices) {
            var repoInterface = service.ServiceType.GetInterfaces().Single(isRepoInterface);
            var factoryInterface = typeof(IRepositoryFactory<,,>)
                .MakeGenericType(
                    new[] {
                            service.ServiceType
                        }.Concat(repoInterface.GetGenericArguments())
                        .ToArray()
                );

            var factory = typeof(RepositoryFactory<,,,>)
                .MakeGenericType(
                    new[] {
                            service.ServiceType,
                            service.ImplementationType!
                        }
                        .Concat(repoInterface.GetGenericArguments())
                        .ToArray()
                );
            serviceCollection.AddSingleton(factoryInterface, factory);
        }

        return serviceCollection;
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей обработчик доменных событий <see cref="DomainEventProcessor" />
    ///     как реализацию интерфейса <see cref="IDomainEventSink" /> и <see cref="IDomainEventDispatcher" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    public static IServiceCollection AddDomainEventProcessor(this IServiceCollection @this) {
        @this.TryAddScoped<DomainEventProcessor>();
        @this.TryAddScoped<IDomainEventSink>(provider => provider.GetRequiredService<DomainEventProcessor>());
        @this.TryAddScoped<IDomainEventDispatcher>(provider => provider.GetRequiredService<DomainEventProcessor>());
        return @this;
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
        @this.TryAddScoped<DomainEventHandlerFactory>();
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces(predicate: IsRegisterInterface)
                .WithTransientLifetime();
        });
    }

    #endregion

    #region Mapster

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей преобразователи,
    ///     реализующие интерфейс <see cref="IEntityMapper{TMainEntity,TId,TDto}" />,
    ///     а также находит конфигурации mapster и регистрирует их в глобальной конфигурации
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <remarks>
    ///     Конфигурация будет проверена (RequireDestinationMemberSource == true) и скомпилирована
    /// </remarks>
    public static IServiceCollection AddEntityMappersAndConfig(this IServiceCollection @this,
        params Assembly[] assemblies) {
        RegisterMapsterConfigs(assemblies);
        return @this.AddEntityMappers(assemblies);
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей преобразователи,
    ///     реализующие интерфейс <see cref="IEntityMapper{TMainEntity,TId,TDto}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    internal static IServiceCollection AddEntityMappers(this IServiceCollection @this, params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IEntityMapper<,,>)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsSelfWithInterfaces(predicate: IsRegisterInterface)
                .WithSingletonLifetime();
        });
    }

    /// <summary>
    ///     Находит конфигурации mapster и регистрирует их в глобальной конфигурации
    ///     <see cref="TypeAdapterConfig.GlobalSettings" />
    /// </summary>
    /// <remarks>
    ///     Конфигурация будет проверена (RequireDestinationMemberSource == true) и скомпилирована
    /// </remarks>
    internal static void RegisterMapsterConfigs(params Assembly[] assemblies) {
        if (_isMapsterConfigured) return;

        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : AppDomain.CurrentDomain.GetAssemblies();

        lock (MapsterLock) {
            if (_isMapsterConfigured) return;

            var configs = TypeAdapterConfig.GlobalSettings.Scan(assembliesToScan);

            // Конфигурация должна охватывать все свойства назначаемого типа
            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;

            TypeAdapterConfig.GlobalSettings.Compile();
            TypeAdapterConfig.GlobalSettings.CompileProjection();

            _isMapsterConfigured = true;
        }
    }

    /// <summary>
    ///     Блокировка конфигурации Mapster (для параллельно выполняемых тестов)
    /// </summary>
    private static readonly object MapsterLock = new();

    /// <summary>
    ///     Сформирована ли конфигурация Mapster (для параллельно выполняемых тестов)
    /// </summary>
    private static bool _isMapsterConfigured;

    #endregion

    #region Бизнес-логика

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей элементы бизнес-логики:
    ///     <list type="bullet">
    ///         <item>Исполнитель команд <see cref="IBusinessActionRunner{TCommand}" /></item>
    ///         <item>Проверяющих команд <see cref="ISpecificationHolder{T}" /></item>
    ///         <item>Проверяющих бизнес-ограничения <see cref="IBusinessConstraintCheck{TCommand}" /></item>
    ///         <item>
    ///             Обработчики бизнес команд
    ///             <see cref="BusinessCommandDbHandler{TCommand,TEntity,TId,TRepo,TLoad,TResult}" />
    ///         </item>
    ///         <item>Обработчики запросов <see cref="IQueryHandler{TQuery,TResult}" /></item>
    ///         <item>Конверторы <see cref="ITriggerEventToCommandTranslator{T}" /></item>
    ///     </list>
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddBusinessLogic(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this
            .AddBusinessActionRunner()
            .AddValidotValidators(assemblies)
            .AddBusinessConstraintsChecks(assemblies)
            .AddBusinessCommandDbHandlers(assemblies)
            .AddQueryHandlers(assemblies)
            .AddGenericQueryHandlers(assemblies)
            .AddTriggerEventToCommandTranslators(assemblies);
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей исполнитель команд <see cref="IBusinessActionRunner{TCommand}" />
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    internal static IServiceCollection AddBusinessActionRunner(this IServiceCollection @this) {
        @this.TryAddScoped<BusinessActionRunnerFactory>();
        @this.TryAddTransient(typeof(IBusinessActionRunner<>), typeof(BusinessActionRunner<>));
        return @this;
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей проверяющих бизнес-ограничения,
    ///     реализующих интерфейс <see cref="IBusinessConstraintCheck{TCommand}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    internal static IServiceCollection AddBusinessConstraintsChecks(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IBusinessConstraintCheck<>)))
                .AsImplementedInterfaces(predicate: IsRegisterInterface)
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
    internal static IServiceCollection AddBusinessCommandDbHandlers(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(BusinessCommandDbHandler<,,,,,>)))
                .AsImplementedInterfaces(predicate: IsRegisterInterface)
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
    internal static IServiceCollection AddQueryHandlers(this IServiceCollection @this,
        params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces(predicate: IsRegisterInterface)
                .WithTransientLifetime();
        });
    }

    /// <summary>
    ///     Регистрирует в фабрике <see cref="QueryHandlerFactory" /> универсальные обработчики запросов,
    ///     реализующие интерфейс <see cref="IQueryHandler{TQuery,TResult}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    internal static IServiceCollection AddGenericQueryHandlers(this IServiceCollection @this,
        params Assembly[] assemblies) {
        @this.TryAddScoped<QueryHandlerFactory>();

        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : AppDomain.CurrentDomain.GetAssemblies();

        var _ = assembliesToScan
            .SelectMany(asm => asm.ExportedTypes)
            .Select(QueryHandlerFactory.TryAddGenericQueryHandler)
            .ToList();


        return @this;
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей конверторы <see cref="TriggerEvent" />,
    ///     реализующие интерфейс <see cref="ITriggerEventToCommandTranslator{T}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    internal static IServiceCollection AddTriggerEventToCommandTranslators(this IServiceCollection @this,
        params Assembly[] assemblies) {
        @this.TryAddScoped<TriggerEventToCommandTranslatorFactory>();

        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(ITriggerEventToCommandTranslator<>)))
                .AsImplementedInterfaces(predicate: IsRegisterInterface)
                .WithTransientLifetime();
        });
    }

    #endregion
}