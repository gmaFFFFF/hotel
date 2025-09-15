using System.Collections.Concurrent;
using System.Linq.Expressions;
using LanguageExt;

namespace gmafffff.starterKit.Utils;

/// <summary>
///     Обеспечивает создание экземпляра класса, определяемого параметром типа в универсальном классе
/// </summary>
public static class Activator<T> {
    public const string ErrorMessage = "Для типа {0} не обнаружен конструктор, принимающий переданные параметры";

    private static readonly ConcurrentDictionary<Arr<Type>, object?> Factories = new();

    /// <summary>
    ///     Создает новый экземпляр типа <typeparamref name="T" />, вызывая конструктор с переданными параметрами
    /// </summary>
    /// <param name="arg">Аргумент, переданный в конструктор</param>
    /// <typeparam name="TParam">Тип параметра конструктора</typeparam>
    /// <remarks>
    ///     Информация о производительности различных методов создания объектов позаимствована из статьи Andrew Lock
    ///     <see href="https://andrewlock.net/benchmarking-4-reflection-methods-for-calling-a-constructor-in-dotnet/">
    ///         Benchmarking
    ///         4 reflection methods for calling a constructor in .NET
    ///     </see>
    ///     По данным его тестов компиляция лямбды занимает в 1_300 раз больше времени, чем вызов
    ///     <see cref="Activator.CreateInstance(Type, object?[]?)" />
    ///     Поэтому пока лямбда не скомпилирована возвращается объект, созданный <see cref="Activator" />
    ///     Правда по моим тестам ко второму вызову лямбда уже скомпилирована… и наверное не нужно было «огород городить»
    /// </remarks>
    /// <returns></returns>
    public static T CreateInstance<TParam>(TParam arg) {
        var key = new[] { typeof(T), typeof(TParam) };
        var keyImmutable = Arr.createRange(key);
        var args = new object[] { arg };

        // Если лямбда скомпилирована, то выполняем её
        if (Factories.TryGetValue(keyImmutable, out var func) && func is not null)
            return ((Func<TParam, T>)func)(arg);

        // Если лямбды нет, то пробуем опередить конкурентов и создать её
        if (Factories.TryAdd(keyImmutable, value: null))
            Task.Run(() => {
                var factory = MakeExpression().Compile();
                Factories.TryUpdate(keyImmutable, factory, comparisonValue: null);
            });

        // Так как лямбды пока ещё нет, то используем Activator.CreateInstance
        if (key[0].GetConstructor(key[1..]) is { } ctor)
            return (T)Activator.CreateInstance(key[0], args);

        // Что-то напутано с параметрами конструктора
        throw new InvalidOperationException(string.Format(ErrorMessage, typeof(T).Name));


        Expression<Func<TParam, T>> MakeExpression() {
            // Подходящий конструктор
            var ctor = key[0].GetConstructor(key[1..]);

            if (ctor is null)
                throw new InvalidOperationException(string.Format(ErrorMessage, typeof(T).Name));

            // Параметры конструктора
            var parameters = key[1..]
                .Select((p, i) => Expression.Parameter(key[i + 1], $"param{i + 1}"))
                .ToArray();

            // Вызов конструктора
            Expression ctorCall = Expression.New(ctor, parameters);

            // Сборка фабричного метода
            return Expression.Lambda<Func<TParam, T>>(ctorCall, parameters);
        }
    }

    /// <summary>
    ///     Создает новый экземпляр типа <typeparamref name="T" />, вызывая конструктор с переданными параметрами
    /// </summary>
    /// <param name="arg1">Первый аргумент, переданный в конструктор</param>
    /// <param name="arg2">Второй аргумент, переданный в конструктор</param>
    /// <typeparam name="TParam1">Тип первого параметра конструктора</typeparam>
    /// <typeparam name="TParam2">Тип второго параметра конструктора</typeparam>
    /// <remarks>
    ///     Информация о производительности различных методов создания объектов позаимствована из статьи Andrew Lock
    ///     <see href="https://andrewlock.net/benchmarking-4-reflection-methods-for-calling-a-constructor-in-dotnet/">
    ///         Benchmarking
    ///         4 reflection methods for calling a constructor in .NET
    ///     </see>
    ///     По данным тестов компиляция лямбды занимает в 1_300 раз больше времени, чем вызов
    ///     <see cref="Activator.CreateInstance(Type, object?[]?)" />
    ///     Поэтому пока лямбда не скомпилирована возвращается объект, созданный <see cref="Activator" />
    ///     Правда по моим тестам ко второму вызову лямбда уже скомпилирована… и наверное не нужно было «огород городить»
    /// </remarks>
    /// <returns></returns>
    public static T CreateInstance<TParam1, TParam2>(TParam1 arg1, TParam2 arg2) {
        var key = new[] { typeof(T), typeof(TParam1), typeof(TParam2) };
        var keyImmutable = Arr.createRange(key);
        var args = new object[] { arg1, arg2 };

        // Если лямбда скомпилирована, то выполняем её
        if (Factories.TryGetValue(keyImmutable, out var func) && func is not null)
            return ((Func<TParam1, TParam2, T>)func)(arg1, arg2);

        // Если лямбды нет, то пробуем опередить другие потоки и создать её
        if (Factories.TryAdd(keyImmutable, value: null))
            Task.Run(() => {
                var factory = MakeExpression().Compile();
                Factories.TryUpdate(keyImmutable, factory, comparisonValue: null);
            });

        // Так как лямбды нет, то используем Activator.CreateInstance
        if (key[0].GetConstructor(key[1..]) is { } ctor)
            return (T)Activator.CreateInstance(key[0], args);

        // Что-то напутано с параметрами конструктора
        throw new InvalidOperationException(string.Format(ErrorMessage, typeof(T).Name));


        Expression<Func<TParam1, TParam2, T>> MakeExpression() {
            // Подходящий конструктор
            var ctor = key[0].GetConstructor(key[1..]);

            if (ctor is null)
                throw new InvalidOperationException(string.Format(ErrorMessage, typeof(T).Name));

            // Параметры конструктора
            var parameters = key[1..]
                .Select((p, i) => Expression.Parameter(key[i + 1], $"param{i + 1}"))
                .ToArray();

            // Вызов конструктора
            Expression ctorCall = Expression.New(ctor, parameters);

            // Сборка фабричного метода
            return Expression.Lambda<Func<TParam1, TParam2, T>>(ctorCall, parameters);
        }
    }

    /// <summary>
    ///     Создает новый экземпляр типа <typeparamref name="T" />, вызывая конструктор с переданными параметрами
    /// </summary>
    /// <param name="arg1">Первый аргумент, переданный в конструктор</param>
    /// <param name="arg2">Второй аргумент, переданный в конструктор</param>
    /// <param name="arg3">Третий аргумент, переданный в конструктор</param>
    /// <typeparam name="T">Создаваемый тип</typeparam>
    /// <typeparam name="TParam1">Тип первого параметра конструктора</typeparam>
    /// <typeparam name="TParam2">Тип второго параметра конструктора</typeparam>
    /// <typeparam name="TParam3">Тип третьего параметра конструктора</typeparam>
    /// <remarks>
    ///     Информация о производительности различных методов создания объектов позаимствована из статьи Andrew Lock
    ///     <see href="https://andrewlock.net/benchmarking-4-reflection-methods-for-calling-a-constructor-in-dotnet/">
    ///         Benchmarking
    ///         4 reflection methods for calling a constructor in .NET
    ///     </see>
    ///     По данным тестов компиляция лямбды занимает в 1_300 раз больше времени, чем вызов
    ///     <see cref="Activator.CreateInstance(Type, object?[]?)" />
    ///     Поэтому пока лямбда не скомпилирована возвращается объект, созданный <see cref="Activator" />
    ///     Правда по моим тестам ко второму вызову лямбда уже скомпилирована… и наверное не нужно было «огород городить»
    /// </remarks>
    /// <returns></returns>
    public static T CreateInstance<TParam1, TParam2, TParam3>(TParam1 arg1, TParam2 arg2, TParam3 arg3) {
        var key = new[] { typeof(T), typeof(TParam1), typeof(TParam2), typeof(TParam3) };
        var keyImmutable = Arr.createRange(key);
        var args = new object[] { arg1, arg2, arg3 };

        // Если лямбда скомпилирована, то выполняем её
        if (Factories.TryGetValue(keyImmutable, out var func) && func is not null)
            return ((Func<TParam1, TParam2, TParam3, T>)func)(arg1, arg2, arg3);

        // Если лямбды нет, то пробуем опередить другие потоки и создать её
        if (Factories.TryAdd(keyImmutable, value: null))
            Task.Run(() => {
                var factory = MakeExpression().Compile();
                Factories.TryUpdate(keyImmutable, factory, comparisonValue: null);
            });

        // Так как лямбды нет, то используем Activator.CreateInstance
        if (key[0].GetConstructor(key[1..]) is { } ctor)
            return (T)Activator.CreateInstance(key[0], args);

        // Что-то напутано с параметрами конструктора
        throw new InvalidOperationException(string.Format(ErrorMessage, typeof(T).Name));


        Expression<Func<TParam1, TParam2, TParam3, T>> MakeExpression() {
            // Подходящий конструктор
            var ctor = key[0].GetConstructor(key[1..]);

            if (ctor is null)
                throw new InvalidOperationException(string.Format(ErrorMessage, typeof(T).Name));

            // Параметры конструктора
            var parameters = key[1..]
                .Select((p, i) => Expression.Parameter(key[i + 1], $"param{i + 1}"))
                .ToArray();

            // Вызов конструктора
            Expression ctorCall = Expression.New(ctor, parameters);

            // Сборка фабричного метода
            return Expression.Lambda<Func<TParam1, TParam2, TParam3, T>>(ctorCall, parameters);
        }
    }
}