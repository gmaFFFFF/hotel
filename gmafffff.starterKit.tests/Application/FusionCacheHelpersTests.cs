using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging;
using LanguageExt.Common;
using ZiggyCreatures.Caching.Fusion;

namespace gmafffff.starterKit.tests.Application;

[TestSubject(typeof(FusionCacheHelpers))]
public class FusionCacheHelpersTests {
    private const int baseDuration = 200;

    private static readonly IQueryHandler<Query<TestDto>, TestDto> Handler =
        Substitute.For<IQueryHandler<Query<TestDto>, TestDto>>();

    private static readonly FusionCacheEntryOptions EntryOptions = new() {
        // Длительность кэширования
        Duration = TimeSpan.FromMilliseconds(baseDuration),
        // Повторно использовать устаревшие записи в кэше при сбое обращения к фабрике
        IsFailSafeEnabled = true,
        // Продление времени жизни устаревших значений
        FailSafeMaxDuration = TimeSpan.FromMilliseconds(baseDuration * 4),
        // Задержка повторных запросов в БД при сбое запроса
        FailSafeThrottleDuration = TimeSpan.Zero,
        // Время обработки запроса фабрикой, после которого будет выдано резервное значение
        FactorySoftTimeout = TimeSpan.FromMilliseconds(baseDuration * 2),
        // При истечении жесткого тайм-аута будет выдано исключение типа SyntheticTimeoutException
        FactoryHardTimeout = TimeSpan.FromMilliseconds(baseDuration * 4)
    };

    private readonly FusionCache _cache;

    public FusionCacheHelpersTests() {
        FusionCacheOptions options = new() {
            DefaultEntryOptions = EntryOptions
        };
        _cache = new FusionCache(options);
    }

    /// <summary>
    ///     Тестовые данные для тестирования запросов, возвращающих один объект, на реальном кэше
    /// </summary>
    public static TheoryData<TestQuery, IEnumerable<Fin<TestDto>>, TimeSpan> DataForCacheTestOnRealCache {
        get {
            TheoryData<TestQuery, IEnumerable<Fin<TestDto>>, TimeSpan> testData = [];
            var repeatCount = 3;
            var delayExpired = EntryOptions.Duration.Add(TimeSpan.FromMilliseconds(20));

            var query1 = new TestQuery(Id: 1, "Первый запрос успешный, остальные берутся из кэша");
            var query2 = new TestQuery(Id: 2, "Все запросы успешные, но безрезультатные");
            var query3 = new TestQuery(Id: 3, "В первом запросе ожидаемая ошибка, а потом всё ок");
            var query4 = new TestQuery(Id: 4, "Второй запрос неожиданно провален, поэтому используется запись из кэша");
            var query5 = new TestQuery(Id: 5, "Кэш устаревает при каждом запросе");

            var error = new Exception();
            var result1 = new TestDto();
            var result3 = new TestDto(3);
            var result4_1 = new TestDto(401);
            var result4_3 = new TestDto(403);
            var result5_1 = new TestDto(501);
            var result5_2 = new TestDto(502);
            var result5_3 = new TestDto(303);

            Handler.RunQueryAsync(query1, Arg.Any<CancellationToken>())
                .Returns(
                    Fin<IList<TestDto>>.Succ([result1]),
                    Fin<IList<TestDto>>.Succ([]),
                    Fin<IList<TestDto>>.Succ([]));
            Handler.RunQueryAsync(query2, Arg.Any<CancellationToken>())
                .Returns(
                    Fin<IList<TestDto>>.Succ([]),
                    Fin<IList<TestDto>>.Succ([]),
                    Fin<IList<TestDto>>.Succ([]));
            Handler.RunQueryAsync(query3, Arg.Any<CancellationToken>())
                .Returns(
                    Fin<IList<TestDto>>.Fail(AppErrorHelper.NewError(AppErrorCode.OperationCancel)),
                    Fin<IList<TestDto>>.Succ([result3]),
                    Fin<IList<TestDto>>.Succ([]));
            Handler.RunQueryAsync(query4, Arg.Any<CancellationToken>())
                .Returns(
                    Fin<IList<TestDto>>.Succ([result4_1]),
                    Fin<IList<TestDto>>.Fail(AppErrorHelper.NewError(error)),
                    Fin<IList<TestDto>>.Succ([result4_3]));
            Handler.RunQueryAsync(query5, Arg.Any<CancellationToken>())
                .Returns(
                    Fin<IList<TestDto>>.Succ([result5_1]),
                    Fin<IList<TestDto>>.Succ([result5_2]),
                    Fin<IList<TestDto>>.Succ([result5_3]));

            testData.Add(query1,
                Enumerable.Repeat(Fin<TestDto>.Succ(result1), repeatCount), TimeSpan.Zero);
            testData.Add(query2,
                Enumerable.Repeat(Fin<TestDto>.Fail(AppErrorHelper.NewError(AppErrorCode.DbNotFound)), repeatCount),
                TimeSpan.Zero);
            testData.Add(query3, [
                Fin<TestDto>.Fail(AppErrorHelper.NewError(AppErrorCode.OperationCancel)),
                Fin<TestDto>.Succ(result3),
                Fin<TestDto>.Succ(result3)
            ], TimeSpan.Zero);
            testData.Add(query4, [
                Fin<TestDto>.Succ(result4_1),
                Fin<TestDto>.Succ(result4_1),
                Fin<TestDto>.Succ(result4_3)
            ], delayExpired);
            testData.Add(query5, [
                Fin<TestDto>.Succ(result5_1),
                Fin<TestDto>.Succ(result5_2),
                Fin<TestDto>.Succ(result5_3)
            ], delayExpired);

            return testData;
        }
    }

    /// <summary>
    ///     Кэширует запросы одиночных объектов на реальном кэше
    /// </summary>
    [Theory]
    [MemberData(nameof(DataForCacheTestOnRealCache))]
    public async Task
        Cache_QuerySingleDto_OnRealCache(TestQuery query, IEnumerable<Fin<TestDto>> data, TimeSpan delay) {
        foreach (var excepted in data) {
            var fact = await _cache.GetOrSetAsync(
                query.Id.ToString(),
                factory: FusionCacheHelpers.GetHandleQuerySingleResultAndCacheFactory(query, Handler));

            fact.IsSucc.Should().Be(excepted.IsSucc);
            fact.IfSucc(result => result.Should().Be(excepted.ThrowIfFail()));
            fact.IfFail(error => error.Should().Be((Error)excepted));

            if (delay != TimeSpan.Zero)
                await Task.Delay(delay);
        }
    }

    /// <summary>
    ///     Кэширует запросы одиночных объектов на реальном кэше
    /// </summary>
    [Theory]
    [MemberData(nameof(DataForCacheTestOnRealCache))]
    public async Task Cache_Query_OnRealCache(TestQuery query, IEnumerable<Fin<TestDto>> data, TimeSpan delay) {
        foreach (var excepted in data) {
            var fact = await _cache.GetOrSetAsync(
                query.Id.ToString(),
                factory: FusionCacheHelpers.GetHandleQueryAndCacheFactory(query, Handler));

            // Чтобы не делать два разных тестовых набора особым образом обрабатываем случай когда нет результата 
            if (excepted.IsFail && ((Error)excepted).Code == (int)AppErrorCode.DbNotFound) {
                fact.IsSucc.Should().BeTrue();
                fact.IfSucc(result => result.Should().BeEmpty());
            }
            else {
                fact.IsSucc.Should().Be(excepted.IsSucc);
                fact.IfSucc(result => result.Should().ContainSingle(r => r == excepted.ThrowIfFail()));
                fact.IfFail(error => error.Should().Be((Error)excepted));
            }

            if (delay != TimeSpan.Zero)
                await Task.Delay(delay);
        }
    }

    /// <summary>
    ///     Кэширует множество объектов на реальном кэше
    /// </summary>
    [Fact]
    public async Task Cache_MultipleDto_OnRealCache() {
        // Arrange
        const string primaryKeyName = "key";
        const string alterKeyName = "alter";
        TestDto[] values = [new(101), new(102)];

        // Act
        await _cache.TrySetAsync(
            primaryKeyName,
            primaryKeyGenerator: v => v.Id.ToString(),
            new Dictionary<string, Func<TestDto, string?>> { [alterKeyName] = val => (val.Id * 10).ToString() },
            values
        );
        var result1 = _cache.TryGet<TestDto>(FusionCacheHelpers.KeyFormat(primaryKeyName, values[0].Id.ToString()));
        var result2 = _cache.TryGet<string>(FusionCacheHelpers.KeyFormat(alterKeyName, (values[1].Id * 10).ToString()));

        // Assert
        result1.HasValue.Should().BeTrue();
        result1.Value.Should().Be(values[0]);

        result2.HasValue.Should().BeTrue();
        result2.Value.Should().Be(FusionCacheHelpers.KeyFormat(primaryKeyName, values[1].Id.ToString()));
    }

    /// <summary>
    ///     <see cref="FusionCacheHelpers.GetHandleQuerySingleResultAndCacheFactory{TDto}"/> возвращает ошибку,
    ///     если запрос вернул несколько Dto
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetHandleQuerySingleResultAndCacheFactory_Error_WhenMultipleDtoFound() {
        // Arrange
        var query = new TestQuery(Id: 10, "Возвратит несколько Dto");
        TestDto[] response = [new(101), new(102)];

        Handler.RunQueryAsync(query, Arg.Any<CancellationToken>())
            .Returns(Fin<IList<TestDto>>.Succ(response));

        // Act
        var fact = await _cache.GetOrSetAsync(
            query.Id.ToString(),
            factory: FusionCacheHelpers.GetHandleQuerySingleResultAndCacheFactory(query, Handler));

        // Assert
        fact.IsSucc.Should().Be(false);
        var err = (Error)fact;
        var exc = err.Exception;
        exc.IsSome.Should().Be(true);
        exc.Map(e => e.Should().BeOfType(typeof(InvalidOperationException)));
    }

    /// <summary>
    ///     <see cref="FusionCacheHelpers.GetHandleQueryAndCacheFactory{TDto}" /> поддерживает возврат нескольких Dto
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetHandleQueryAndCacheFactory_Ok_WhenMultipleDtoFound() {
        // Arrange
        var query = new TestQuery(Id: 10, "Возвратит несколько Dto");
        TestDto[] response = [new(101), new(102)];

        Handler.RunQueryAsync(query, Arg.Any<CancellationToken>())
            .Returns(Fin<IList<TestDto>>.Succ(response));

        // Act
        var fact = await _cache.GetOrSetAsync(
            query.Id.ToString(),
            factory: FusionCacheHelpers.GetHandleQueryAndCacheFactory(query, Handler));

        // Assert
        fact.IsSucc.Should().Be(true);
        fact.IfSucc(result => result.Should().Equal(response));
    }

    /// <summary>
    ///     Автоматически сохраняет альтернативные ключи при загрузке
    /// </summary>
    [Fact]
    public async Task AutoSave_AlterKeys() {
        // Arrange
        const string primaryKeyName = "key";
        const string alterKeyName = "alter";
        // Act
        var entity = await _cache.GetOrSetAsync(
            primaryKeyName,
            new Dictionary<string, Func<TestDto, string>> { [alterKeyName] = val => (val.Id * 10).ToString() },
            new TestDto()
        );
        var result = _cache.TryGet<string>(FusionCacheHelpers.KeyFormat(alterKeyName, (entity.Id * 10).ToString()));

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(primaryKeyName);
    }

    public record TestQuery(int Id = 1, string Msq = "None") : Query<TestDto>;

    public record TestDto(int Id = 1, DateTimeOffset Modified = default);
}