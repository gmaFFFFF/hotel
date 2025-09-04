using System.Runtime.CompilerServices;
using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace ZiggyCreatures.Caching.Fusion;

public static partial class FusionCacheHelpers {
    #region Кэширование сущностей с несколькими ключами

    /// <summary>
    ///     Форматирует ключи для кэша
    /// </summary>
    public static Func<string, string, string> KeyFormat = (keyName, keyVal) => $"{keyName}:{keyVal}";

    /// <summary>
    ///     Get the value of type <typeparamref name="TValue" /> in the cache for the specified <paramref name="primaryKey" />:
    ///     if not there, the <paramref name="factory" /> will be called and the returned value saved according to the
    ///     <paramref name="options" /> provided.
    ///     Из полученный сущности генерирует альтернативные ключи с помощью функции <see cref="KeyFormat" />(altKeyName,
    ///     generatorFunc(value)).
    ///     Альтернативные ключи добавляются в кэш со ссылкой на основной ключ
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the cache.</typeparam>
    /// <param name="primaryKey">The cache key which identifies the entry in the cache.</param>
    /// <param name="altKeyGenerators">
    ///     Генератор альтернативных ключей. Ключ словаря — название ключа кэша (altKeyName),
    ///     значение — функция генератор идентификатора (generatorFunc)
    /// </param>
    /// <param name="factory">The function which will be called if the value is not found in the cache.</param>
    /// <param name="failSafeDefaultValue">
    ///     In case fail-safe is activated and there's no stale data to use, this value will be
    ///     used instead of throwing an exception.
    /// </param>
    /// <param name="options">
    ///     The options to adhere during this operation. If null is passed,
    ///     <see cref="DefaultEntryOptions" /> will be used.
    /// </param>
    /// <param name="tags">
    ///     The optional set of tags related to the entry: this may be used to remove/expire multiple entries at
    ///     once, by tag.
    /// </param>
    /// <param name="token">An optional <see cref="CancellationToken" /> to cancel the operation.</param>
    /// <returns>The value in the cache, either already there or generated using the provided <paramref name="factory" /> .</returns>
    public static async ValueTask<TValue> GetOrSetAsync<TValue>(this IFusionCache @this,
        string primaryKey, Dictionary<string, Func<TValue, string?>> altKeyGenerators,
        Func<FusionCacheFactoryExecutionContext<TValue>, CancellationToken, Task<TValue>> factory,
        MaybeValue<TValue> failSafeDefaultValue = default,
        FusionCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken token = default) {
        var result = await @this.GetOrSetAsync(primaryKey, factory, failSafeDefaultValue, options, tags, token)
            .ConfigureAwait(false);
        @this.SetAlterKeysRefToPrimaryKey(primaryKey, altKeyGenerators, result, options, tags, token);

        return result;
    }

    /// <summary>
    ///     Get the value of type <typeparamref name="TValue" /> in the cache for the specified <paramref name="primaryKey" />:
    ///     if not there, the <paramref name="defaultValue" /> will be saved according to the <paramref name="options" />
    ///     provided.
    ///     Из полученный сущности генерирует альтернативные ключи с помощью функции <see cref="KeyFormat" />(altKeyName,
    ///     generatorFunc(value)).
    ///     Альтернативные ключи добавляются в кэш со ссылкой на основной ключ
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the cache.</typeparam>
    /// <param name="primaryKey">The cache key which identifies the entry in the cache.</param>
    /// <param name="altKeyGenerators">
    ///     Генератор альтернативных ключей. Ключ словаря — название ключа кэша (altKeyName),
    ///     значение — функция генератор идентификатора (generatorFunc)
    /// </param>
    /// <param name="defaultValue">In case the value is not in the cache this value will be saved and returned instead.</param>
    /// <param name="options">
    ///     The options to adhere during this operation. If null is passed,
    ///     <see cref="DefaultEntryOptions" /> will be used.
    /// </param>
    /// <param name="tags">
    ///     The optional set of tags related to the entry: this may be used to remove/expire multiple entries at
    ///     once, by tag.
    /// </param>
    /// <param name="token">An optional <see cref="CancellationToken" /> to cancel the operation.</param>
    public static async ValueTask<TValue> GetOrSetAsync<TValue>(this IFusionCache @this,
        string primaryKey, Dictionary<string, Func<TValue, string?>> altKeyGenerators,
        TValue defaultValue,
        FusionCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken token = default) {
        var result = await @this.GetOrSetAsync(primaryKey, defaultValue, options, tags, token).ConfigureAwait(false);
        @this.SetAlterKeysRefToPrimaryKey(primaryKey, altKeyGenerators, result, options, tags, token);

        return result;
    }

    /// <summary>
    ///     Put the <paramref name="value" /> in the cache for the specified <paramref name="primaryKey" />, optionally tagged
    ///     with the specified <paramref name="tags" />, with the provided <paramref name="options" />. If a value is already
    ///     there it will be overwritten.
    ///     Из полученный сущности генерирует альтернативные ключи с помощью функции <see cref="KeyFormat" />(altKeyName,
    ///     generatorFunc(value)).
    ///     Альтернативные ключи добавляются в кэш со ссылкой на основной ключ
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the cache.</typeparam>
    /// <param name="primaryKey">The cache key which identifies the entry in the cache.</param>
    /// <param name="altKeyGenerators">
    ///     Генератор альтернативных ключей. Ключ словаря — название ключа кэша (altKeyName),
    ///     значение — функция генератор идентификатора (generatorFunc)
    /// </param>
    /// <param name="value">The value to put in the cache.</param>
    /// <param name="options">
    ///     The options to adhere during this operation. If null is passed,
    ///     <see cref="DefaultEntryOptions" /> will be used.
    /// </param>
    /// <param name="tags">
    ///     The optional set of tags related to the entry: this may be used to remove/expire multiple entries at
    ///     once, by tag.
    /// </param>
    /// <param name="token">An optional <see cref="CancellationToken" /> to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask" /> to await the completion of the operation.</returns>
    public static async ValueTask SetAsync<TValue>(this IFusionCache @this,
        string primaryKey, Dictionary<string, Func<TValue, string?>> altKeyGenerators,
        TValue value,
        FusionCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken token = default) {
        @this.SetAlterKeysRefToPrimaryKey(primaryKey, altKeyGenerators, value, options, tags, token);
        await @this.SetAsync(primaryKey, value, options, tags, token).ConfigureAwait(false);
    }

    /// <summary>
    ///     Put the <paramref name="value" /> in the cache for the specified <paramref name="primaryKey" /> with the provided
    ///     <paramref name="options" />. If a value is already there it will be overwritten.
    ///     Из полученный сущности генерирует альтернативные ключи с помощью функции <see cref="KeyFormat" />(altKeyName,
    ///     generatorFunc(value)).
    ///     Альтернативные ключи добавляются в кэш со ссылкой на основной ключ
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the cache.</typeparam>
    /// <param name="primaryKey">The cache key which identifies the entry in the cache.</param>
    /// <param name="altKeyGenerators">
    ///     Генератор альтернативных ключей. Ключ словаря — название ключа кэша (altKeyName),
    ///     значение — функция генератор идентификатора (generatorFunc)
    /// </param>
    /// <param name="value">The value to put in the cache.</param>
    /// <param name="options">
    ///     The options to adhere during this operation. If null is passed,
    ///     <see cref="DefaultEntryOptions" /> will be used.
    /// </param>
    /// <param name="tags">
    ///     The optional set of tags related to the entry: this may be used to remove/expire multiple entries at
    ///     once, by tag.
    /// </param>
    /// <param name="token">An optional <see cref="CancellationToken" /> to cancel the operation.</param>
    public static void Set<TValue>(this IFusionCache @this,
        string primaryKey, Dictionary<string, Func<TValue, string?>> altKeyGenerators,
        TValue value,
        FusionCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken token = default) {
        @this.SetAlterKeysRefToPrimaryKey(primaryKey, altKeyGenerators, value, options, tags, token);
        @this.Set(primaryKey, value, options, tags, token);
    }


    /// <summary>
    ///     Кэширует ссылки альтернативных ключей на первичный ключ.
    ///     Альтернативные ключи генерируются с помощью функции <see cref="KeyFormat" />(altKeyName, generatorFunc(value))
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the cache.</typeparam>
    /// <param name="primaryKey">The cache key which identifies the entry in the cache.</param>
    /// <param name="altKeyGenerators">
    ///     Генератор альтернативных ключей. Ключ словаря — название ключа кэша (altKeyName),
    ///     значение — функция генератор идентификатора (generatorFunc)
    /// </param>
    /// <param name="value">Значение, помещенное в кэш</param>
    /// <param name="options">
    ///     The options to adhere during this operation. If null is passed,
    ///     <see cref="DefaultEntryOptions" /> will be used.
    /// </param>
    /// <param name="tags">
    ///     The optional set of tags related to the entry: this may be used to remove/expire multiple entries at
    ///     once, by tag.
    /// </param>
    /// <param name="token">An optional <see cref="CancellationToken" /> to cancel the operation.</param>
    internal static void SetAlterKeysRefToPrimaryKey<TValue>(this IFusionCache @this,
        string primaryKey, Dictionary<string, Func<TValue, string?>> altKeyGenerators,
        TValue value,
        FusionCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken token = default) {
        foreach (var (altKeyName, generator) in altKeyGenerators)
            if (generator(value) is { } altKey)
                @this.Set(KeyFormat(altKeyName, altKey), primaryKey, options, tags, token);
    }


    #region Кэширование множества значений

    /// <summary>
    ///     Помещает в кэш несколько <paramref name="values" />, с предоставленными <paramref name="options" />.
    ///     Значение основного ключа извлекает из объекта <paramref name="primaryKeyGenerator" />, из альтернативного —
    ///     <paramref name="altKeyGenerators" />
    ///     Ключи форматируются с помощью функции <see cref="KeyFormat" />(keyName, keyValue).
    ///     Если значение уже добавлено, то оно будет перезаписано.
    ///     Альтернативные ключи добавляются в кэш со ссылкой на основной ключ
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the cache.</typeparam>
    /// <param name="primaryKeyName">Название ключа.</param>
    /// <param name="primaryKeyGenerator">Генератор основного ключа.</param>
    /// <param name="altKeyGenerators">
    ///     Генератор альтернативных ключей. Ключ словаря — название ключа кэша (altKeyName),
    ///     значение — функция генератор идентификатора (generatorFunc)
    /// </param>
    /// <param name="values">The values to put in the cache.</param>
    /// <param name="getLastModified">
    ///     Функция, определяющая дату модификации элемента.
    ///     По умолчанию <see cref="DateTimeOffset.UtcNow" />
    /// </param>
    /// <param name="options">
    ///     The options to adhere during this operation. If null is passed,
    ///     <see cref="DefaultEntryOptions" /> will be used.
    /// </param>
    /// <param name="tags">
    ///     The optional set of tags related to the entry: this may be used to remove/expire multiple entries at
    ///     once, by tag.
    /// </param>
    /// <param name="token">An optional <see cref="CancellationToken" /> to cancel the operation.</param>
    public static void Set<TValue>(this IFusionCache @this,
        string primaryKeyName, Func<TValue, string> primaryKeyGenerator,
        Dictionary<string, Func<TValue, string?>> altKeyGenerators,
        ICollection<TValue> values,
        Func<TValue, DateTimeOffset>? getLastModified = null,
        FusionCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken token = default) {
        foreach (var value in values) {
            var primaryKey = KeyFormat(primaryKeyName, primaryKeyGenerator(value));
            @this.SetAlterKeysRefToPrimaryKey(primaryKey, altKeyGenerators, value, options, tags, token);
            @this.GetOrSet<TValue>(primaryKey, factory: (context, cancel) => {
                var lastModified = getLastModified?.Invoke(value) ?? DateTimeOffset.UtcNow;
                return (context.HasStaleValue, context.HasLastModified) switch {
                    (true, true) when context.LastModified!.Value >= lastModified
                        => context.NotModified(),
                    _ => context.Modified(value, lastModified: lastModified)
                };
            }, options, tags, token);
            @this.Set(primaryKey, value, options, tags, token);
        }
    }

    #endregion

    #endregion

    #region Интеграция обработчиков IQueryHandler{TQuery,TResult}

    /// <summary>
    ///     Выполняет запрос к БД, предусматривающий возврат единственного элемента, и кэширует его результат
    /// </summary>
    /// <param name="query">Запрос, предусматривающий возврат единственного элемента</param>
    /// <param name="handler">Обработчик запроса</param>
    /// <param name="context">Контекст выполнения фабрики FusionCache</param>
    /// <param name="cancel">Токен отмены</param>
    /// <param name="getLastModified">
    ///     Функция, определяющая дату модификации элемента.
    ///     По умолчанию <see cref="DateTimeOffset.UtcNow" />
    /// </param>
    /// <param name="logger">Журнал</param>
    /// <typeparam name="TDto">Тип возвращаемого элемента</typeparam>
    /// <remarks>
    ///     Обработка ошибок:
    ///     <list type="bullet">
    ///         <item>Инфраструктурные ошибки (IsExceptional) — дать возможность использовать резервное значение</item>
    ///         <item>Ожидаемые ошибки (Excepted) — не кэшируются и возвращаются как есть</item>
    ///         <item>Не найден объект — результат не кэшируются и возвращает ошибку</item>
    ///         <item>Некорректный запрос, возвращающий > 1 записи — не кэшируются и возвращает ошибку</item>
    ///     </list>
    /// </remarks>
    /// <returns></returns>
    internal static async Task<Fin<TDto>> QuerySingleDtoAndCacheFabric<TDto>(
        Query<TDto> query, IQueryHandler<Query<TDto>, TDto> handler,
        FusionCacheFactoryExecutionContext<Fin<TDto>> context, CancellationToken cancel = default,
        Func<TDto, DateTimeOffset>? getLastModified = null,
        ILogger? logger = null) {
        // 1. Загрузка данных из БД
        var timestamp = DateTimeOffset.UtcNow;
        var loaded = await handler.RunQueryAsync(query, cancel).ConfigureAwait(false);

        // 2. Проверка работоспособности инфраструктуры
        if (loaded.IsFail) {
            var error = (Error)loaded;
            logger?.QueryFailed(query, error);

            // Если произошла какая-то неожиданная ошибка (ошибка инфраструктуры), то можно брать резервное значение
            if (error.AsIterable().Any(err => err.IsExceptional))
                return context.Fail(error.ToString());

            // Если ошибка ожидаемая, то её нужно передать потребителю, но НЕ кэшировать результат
            context.Options.SetSkipCacheWrite();
            return loaded.Map(dtos => dtos.Single()); // Map не сработает, так как `loaded.IsFail`, но тип будет TDto
        }

        // 3. Проверка на ошибку в запросе. Правильный запрос должен вернуть только один объект 
        var find = loaded.ThrowIfFail();
        switch (find.Count) {
            case 1:
                var result = loaded.Map(dtos => dtos.Single());
                var value = result.ThrowIfFail();
                var lastModified = getLastModified?.Invoke(value) ?? timestamp;
                logger?.QuerySuccess(query, value);

                return (context.HasStaleValue, context.HasLastModified) switch {
                    (true, true) when context.LastModified!.Value >= lastModified
                        => context.NotModified(),
                    _ => context.Modified(result, lastModified: lastModified)
                };

            case 0:
                context.Options.SetSkipCacheWrite();
                logger?.NotFound(query);
                return AppErrorHelper.NewError(AppErrorCode.DbNotFound);

            default: // case > 1:
                context.Options.SetSkipCacheWrite();
                logger?.InvalidResultCount(query, find.Count, find.Cast<object>());
                return AppErrorHelper.NewError(new InvalidOperationException(
                    $"Найдено {find.Count} записи(ей), а ожидалась одна"));
        }
    }

    /// <summary>
    ///     Вспомогательный метод, отключающий кэширование в памяти и распределённом кэше
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private static FusionCacheEntryOptions SetSkipCacheWrite(this FusionCacheEntryOptions options) {
        return options
            .SetSkipDistributedCacheWrite(skip: true, skipBackplaneNotifications: null)
            .SetSkipMemoryCacheWrite();
    }

    /// <summary>
    ///     Возвращает функцию, вызываемую методами <see cref="IFusionCache" />, в случае отсутствия значения в кэше,
    ///     для запросов единичного значения с помощью <see cref="IQueryHandler{TQuery,TResult}" />
    /// </summary>
    /// <param name="query">Запрос, предусматривающий возврат единственного элемента</param>
    /// <param name="handler">Обработчик запроса</param>
    /// <param name="getLastModified">
    ///     Функция, определяющая дату модификации элемента.
    ///     По умолчанию <see cref="DateTimeOffset.UtcNow" />
    /// </param>
    /// <param name="logger">Журнал</param>
    /// <typeparam name="TDto">Тип возвращаемого элемента</typeparam>
    /// <returns></returns>
    public static Func<FusionCacheFactoryExecutionContext<Fin<TDto>>, CancellationToken, Task<Fin<TDto>>>
        GetQuerySingleDtoAndCacheFabric<TDto>(Query<TDto> query, IQueryHandler<Query<TDto>, TDto> handler,
            Func<TDto, DateTimeOffset>? getLastModified = null,
            ILogger? logger = null) {
        Func<Query<TDto>, IQueryHandler<Query<TDto>, TDto>,
            FusionCacheFactoryExecutionContext<Fin<TDto>>,
            CancellationToken,
            Func<TDto, DateTimeOffset>?,
            ILogger?,
            Task<Fin<TDto>>> targetFunc = QuerySingleDtoAndCacheFabric<TDto>;

        var reorderFuncArg = ReorderFuncArg(targetFunc);
        var partial = par(reorderFuncArg, query, handler, getLastModified, logger);

        return partial;
    }

    /// <summary>
    ///     Изменяет порядок аргументов функции
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Func<T1, T2, T5, T6, T3, T4, TResult> ReorderFuncArg<T1, T2, T3, T4, T5, T6, TResult>(
        Func<T1, T2, T3, T4, T5, T6, TResult> func) {
        return (a, b, e, f, c, d) => func(a, b, c, d, e, f);
    }

    #endregion

    #region Логирование

    /// <summary>
    ///     CRC16 для <see cref="ZiggyCreatures.Caching.Fusion.FusionCacheHelpers" />
    /// </summary>
    public const int EventIdBase = 0x5626;

    [LoggerMessage(EventId = EventIdBase + 1, Level = LogLevel.Error,
        Message = "Запрос {@Query} должен вернуть 1 запись, но получено {Count} записи(ей): {@Received}")]
    private static partial void InvalidResultCount(this ILogger logger, Message query, int count,
        IEnumerable<object> received);

    [LoggerMessage(EventId = EventIdBase + 2, Level = LogLevel.Warning,
        Message = "Ошибка выполнения запроса {@Query}: {@Error}")]
    private static partial void QueryFailed(this ILogger logger, Message query, Error error);

    [LoggerMessage(EventId = EventIdBase + 3, Level = LogLevel.Debug,
        Message = "Объект по запросу {@Query} не найден")]
    private static partial void NotFound(this ILogger logger, Message query);

    [LoggerMessage(EventId = EventIdBase + 4, Level = LogLevel.Trace,
        Message = "Запросу {@Query} вернул объект для кэширования: {@Cache}")]
    private static partial void QuerySuccess(this ILogger logger, Message query, object cache);

    #endregion
}