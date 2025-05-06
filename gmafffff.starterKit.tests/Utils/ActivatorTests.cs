using gmafffff.starterKit.Utils;

namespace gmafffff.starterKit.Tests.Utils;

[TestSubject(typeof(Activator<>))]
public class ActivatorTests {
    public const int RepeatCount = 10;

    /// <summary>
    ///     Можно создать универсальный тип с однопараметрическим конструктором
    /// </summary>
    /// <remarks>
    ///     Проверять, что используется именно скомпилированная лямбда, нужно вручную через отладчик после строчки
    ///     <code>if (Factories.TryGetValue(keyImmutable, out var func) && func is not null)<code>
    /// </remarks>
    [Fact]
    public void CanCreateGenericTypeWithSingleParametricConstructor() {
        for (var i = 1; i < RepeatCount; i++) {
            // Arrange
            var excepted = new Test1<int>(i);

            // Act
            var res = Create<int, Test1<int>>(i);

            // Assert
            res.Should().Be(excepted);
        }

        T Create<TParam1, T>(TParam1 arg1) {
            return Activator<T>.CreateInstance(arg1);
        }
    }

    /// <summary>
    ///     Можно создать универсальный тип с двупараметрическим конструктором
    /// </summary>
    /// <remarks>
    ///     Проверять, что используется именно скомпилированная лямбда, нужно вручную через отладчик после строчки
    ///     <code>if (Factories.TryGetValue(keyImmutable, out var func) && func is not null)<code>
    /// </remarks>
    [Fact]
    public void CanCreateGenericTypeWithDoubleParametricConstructor() {
        for (var i = 1; i < RepeatCount; i++) {
            // Arrange
            var excepted = new Test2<int, string>(i, i.ToString());

            // Act
            var res = Create<int, string, Test2<int, string>>(i, i.ToString());

            // Assert
            res.Should().Be(excepted);
        }

        T Create<TParam1, TParam2, T>(TParam1 arg1, TParam2 arg2) {
            return Activator<T>.CreateInstance(arg1, arg2);
        }
    }

    /// <summary>
    ///     Можно создать универсальный тип с трех-параметрическим конструктором
    /// </summary>
    /// <remarks>
    ///     Проверять, что используется именно скомпилированная лямбда, нужно вручную через отладчик после строчки
    ///     <code>if (Factories.TryGetValue(keyImmutable, out var func) && func is not null)<code>
    /// </remarks>
    [Fact]
    public void CanCreateGenericTypeWithTripleParametricConstructor() {
        for (var i = 1; i < RepeatCount; i++) {
            // Arrange
            var excepted = new Test3<int, string, float>(i, i.ToString(), i);

            // Act
            var res = Create<int, string, float, Test3<int, string, float>>(i, i.ToString(), i);

            // Assert
            res.Should().Be(excepted);
        }

        T Create<TParam1, TParam2, TParam3, T>(TParam1 arg1, TParam2 arg2, TParam3 arg3) {
            return Activator<T>.CreateInstance(arg1, arg2, arg3);
        }
    }
}

file record Test1<TParam1>(TParam1 arg1);

file record Test2<TParam1, TParam2>(TParam1 arg1, TParam2 arg2);

file record Test3<TParam1, TParam2, TParam3>(TParam1 arg1, TParam2 arg2, TParam3 arg3);