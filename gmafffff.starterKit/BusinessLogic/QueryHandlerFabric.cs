using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.Messaging.Crud;
using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     В DI контейнере Microsoft нельзя осуществить сложную регистрацию открытых обобщённых типов.
///     Это делает невозможным регистрацию универсального <see cref="IQueryHandler{TQuery,TResult}" />,
///     TResult которого может подбираться компилятором в зависимости от TDto из <see cref="ReadDbQuery{TDto}" />
///     или <see cref="ReadDbQuery{TEntity, TDto}" />.
///     Поэтому для каждого TResult, возвращаемого <see cref="IQueryHandler{TQuery,TResult}" />,
///     нужно создать отдельный обработчик или воспользоваться этой фабрикой.
/// </summary>
public record QueryHandlerFactory(IServiceProvider ServiceProvider) {
    /// <summary>
    ///     Словарь, где ключом является тип запроса <see cref="Query{T}" />,
    ///     а значением универсальный тип его обработчика <see cref="IQueryHandler{TQuery,TResult}" />
    /// </summary>
    internal static readonly Dictionary<Type, Type> GenericHandlers = [];

    private static readonly object LockGenericHandlers = new();

    /// <summary>
    ///     Возвращает <see cref="IQueryHandler{TQuery,TResult}" /> для определённого запроса <typeparamref name="TQuery" />.
    ///     Запрос можно сделать передав аргументы-подсказки, позволяющие определить тип запроса:
    ///     <paramref name="queryTypeHint" /> и <paramref name="dtoTypeHint" />,
    ///     или задав типы параметра <typeparamref name="TQuery" /> и <typeparamref name="TDto" />
    /// </summary>
    /// <param name="queryTypeHint">Объект подсказка для вывода типа <typeparamref name="TQuery" /></param>
    /// <param name="dtoTypeHint">Объект подсказка для вывода типа <typeparamref name="TDto" /></param>
    /// <typeparam name="TQuery">
    ///     Тип запроса, например, <see cref="ReadDbQuery{TDto}" /> или <see cref="ReadDbQuery{TEntity,TDto}" />
    /// </typeparam>
    /// <typeparam name="TDto">Тип результата, возвращаемый запросом</typeparam>
    /// <remarks>
    ///     К сожалению компилятор C# не может "заглянуть" в ограничение generic-типа TQuery для вывода типа TDto.
    ///     Поэтому при использовании аргументов-подсказок нужно передать не только <paramref name="queryTypeHint" />,
    ///     но и <paramref name="dtoTypeHint" />.
    ///     Допустимо передавать не реальные объекты, а созданные оператором default
    /// </remarks>
    /// <returns>Зарегистрированный обработчик запроса</returns>
    public IQueryHandler<TQuery, TDto>? GetQueryHandler<TQuery, TDto>(
        TQuery queryTypeHint = null!, TDto dtoTypeHint = default!)
        where TQuery : Query<TDto> {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TDto));
        var handler = (IQueryHandler<TQuery, TDto>?)ServiceProvider.GetService(handlerType);
        if (handler is not null)
            return handler;

        var find = GenericHandlers.TryGetValue(typeof(TQuery).GetGenericTypeDefinition(), out var genericHandler);
        if (!find)
            return null;

        var constructedGeneric = genericHandler!.MakeGenericType(typeof(TDto));
        return (IQueryHandler<TQuery, TDto>?)ActivatorUtilities.CreateInstance(ServiceProvider, constructedGeneric);
    }

    /// <summary>
    ///     Пробует зарегистрировать подходящий обработчик типа <see cref="IQueryHandler{TQuery,TResult}" />.
    ///     Метод относительно безопасный — сам отфильтрует неподходящий тип.
    /// </summary>
    /// <param name="handler">Тип потенциального обработчика</param>
    /// <returns>True — если обработчик принят, false — в противном случае</returns>
    public static bool TryAddGenericQueryHandler(Type handler) {
        if (!handler.IsGenericType ||
            handler.IsConstructedGenericType ||
            handler.IsAbstract ||
            handler.GetGenericArguments().Length > 1 ||
            !IsGenericRealizeInterface(handler, typeof(IQueryHandler<,>)))
            return false;

        var query = ExtractQuery(handler).GetGenericTypeDefinition();

        lock (LockGenericHandlers) {
            return GenericHandlers.TryAdd(query, handler);
        }

        static bool IsGenericRealizeInterface(Type? testType, Type @interface) {
            if (testType is null || !@interface.IsInterface)
                return false;

            return testType
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .Select(i => i.IsConstructedGenericType ? i.GetGenericTypeDefinition() : i)
                .Contains(@interface);
        }

        static bool IsGenericSubclassOf(Type? testType, Type baseType) {
            if (testType is null || testType == typeof(object))
                return false;

            if (testType.IsConstructedGenericType)
                testType = testType.GetGenericTypeDefinition();

            return testType == baseType || IsGenericSubclassOf(testType.BaseType, baseType);
        }

        static Type ExtractQuery(Type type) {
            ArgumentNullException.ThrowIfNull(type);
            var args = type.GetGenericArguments().Where(arg => IsGenericSubclassOf(arg, typeof(Query<>)));
            return args.FirstOrDefault() ?? ExtractQuery(type.BaseType);
        }
    }
}