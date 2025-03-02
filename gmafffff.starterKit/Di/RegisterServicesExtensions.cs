using System.Reflection;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Validot;

namespace gmafffff.starterKit.Di;

public static class RegisterServicesExtensions {
    /// <summary>
    /// Служебные интерфейсы, которые не требуется регистрировать
    /// </summary>
    private static Type[] _ignoreInterfaces = [
        typeof(IEntityMapper<,,>),
        typeof(IEntityMapperForward<,,>),
        typeof(IEntityMapperBackward<,,>),
        typeof(IEntityMapperDuplex<,,>),
        typeof(IEntityMapperForwardExpression<,,>),
        typeof(IRepository<,>),
        typeof(IDisposable),
        typeof(IAsyncDisposable),
    ];

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей проверяющих бизнес-правила,
    ///     реализующих интерфейс <see cref="IBusinessRule{TCommand}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddBusinessRules(this IServiceCollection @this, params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IBusinessRule<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей репозитории,
    ///     реализующие интерфейс <see cref="IRepository{T,TId}" />
    /// </summary>
    /// <param name="this">Описание служб</param>
    /// <param name="assemblies">Сборки для поиска. Если аргумент опущен, то поиск по всем сборкам домена приложения</param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection @this, params Assembly[] assemblies) {
        return @this.Scan(scan => {
            var selector = assemblies.Length == 0
                ? scan.FromApplicationDependencies()
                : scan.FromAssemblies(assemblies);

            selector
                .AddClasses(@class => @class.AssignableTo(typeof(IRepository<,>)))
                .AsImplementedInterfaces(predicate: @interface =>
                    @interface.IsGenericType
                        ? !_ignoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                        : !_ignoreInterfaces.Contains(@interface))
                .WithScopedLifetime();
        });
    }

    /// <summary>
    ///     Регистрирует в сервисе внедрения зависимостей обработчики бизнес команд,
    ///     реализующие класс <see cref="BusinessCommandDbHandler{TCommand,TEvent,TResult}" />
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
                .AddClasses(@class => @class.AssignableTo(typeof(BusinessCommandDbHandler<,,>)))
                .AsImplementedInterfaces(predicate: @interface =>
                    @interface.IsGenericType
                        ? !_ignoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                        : !_ignoreInterfaces.Contains(@interface))
                .WithTransientLifetime();
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
                        ? !_ignoreInterfaces.Contains(@interface.GetGenericTypeDefinition())
                        : !_ignoreInterfaces.Contains(@interface))
                .WithSingletonLifetime();
        });
    }

    /// <summary>
    /// Находит конфигурации mapster и регистрирует их в глобальной конфигурации
    /// <see cref="TypeAdapterConfig.GlobalSettings"/>
    /// </summary>
    public static void RegisterMapsterConfigs(params Assembly[] assemblies) {
        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : AppDomain.CurrentDomain.GetAssemblies();

        TypeAdapterConfig.GlobalSettings.Scan(assembliesToScan);
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
}