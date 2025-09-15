using FluentAssertions.Execution;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Di;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.tests.Di.Fixtures;

namespace gmafffff.starterKit.tests.Di;

[TestSubject(typeof(QueryHandlerFactory))]
[TestSubject(typeof(RegisterServicesExtensions))]
public class QueryHandlerFactoryTests {
    private readonly TestReadDbQueryDtoHandler _dtoHandler = new(null!, null!);
    private readonly TestReadDbQueryEntHandler _entHandler = new(null!, null!);
    private readonly IServiceProvider _provider;

    public QueryHandlerFactoryTests() {
        _provider = Substitute.For<IServiceProvider>();
        _provider.GetService(typeof(IQueryHandler<TestReadDbQueryDto, TestDto>))
            .Returns(_dtoHandler);
        _provider.GetService(typeof(IQueryHandler<TestReadDbQueryEnt, TestDto>))
            .Returns(_entHandler);
        _provider.GetService(typeof(IQueryHandler<TestGenericReadDbQueryDto<TestDto2>, TestDto2>))
            .Returns(null);
        _provider.GetService(typeof(IQueryHandler<TestGenericReadDbQueryEnt<TestDto2>, TestDto2>))
            .Returns(null);
        _provider.GetService(typeof(IRepositoryReadOnly<TestEntity, int>))
            .Returns(Substitute.For<IRepositoryReadOnly<TestEntity, int>>());
        _provider.GetService(typeof(IEntityMapperForwardExpression<TestEntity, int, TestDto2>))
            .Returns(Substitute.For<IEntityMapperForwardExpression<TestEntity, int, TestDto2>>());
    }

    [Fact]
    public void Factory_Get_NotGenericHandlers() {
        // Arrange
        var factory = new QueryHandlerFactory(_provider);
        var query1 = new TestReadDbQueryDto();
        var query2 = new TestReadDbQueryEnt();

        // Act
        var handler1 = factory.GetQueryHandler(query1, default(TestDto)!);
        var handler2 = factory.GetQueryHandler(query2, default(TestDto)!);

        // Assert
        using var _ = new AssertionScope();
        handler1.Should().Be(_dtoHandler);
        handler2.Should().Be(_entHandler);
    }

    [Fact]
    public void Factory_Add_GenericHandlers() {
        // Arrange, Act
        QueryHandlerFactory.TryAddGenericQueryHandler(typeof(TestGenericReadDbQueryDtoHandler<>));
        QueryHandlerFactory.TryAddGenericQueryHandler(typeof(TestGenericReadDbQueryEntHandler<>));

        // Assert
        using var _ = new AssertionScope();
        QueryHandlerFactory.GenericHandlers.Should().HaveCount(2);
        QueryHandlerFactory.GenericHandlers.Should().ContainKey(typeof(TestGenericReadDbQueryDto<>))
            .WhoseValue.Should().Be(typeof(TestGenericReadDbQueryDtoHandler<>));
        QueryHandlerFactory.GenericHandlers.Should().ContainKey(typeof(TestGenericReadDbQueryEnt<>))
            .WhoseValue.Should().Be(typeof(TestGenericReadDbQueryEntHandler<>));
    }


    [Fact]
    public void Factory_Get_GenericHandlers() {
        // Arrange
        QueryHandlerFactory.TryAddGenericQueryHandler(typeof(TestGenericReadDbQueryDtoHandler<>));
        QueryHandlerFactory.TryAddGenericQueryHandler(typeof(TestGenericReadDbQueryEntHandler<>));
        var factory = new QueryHandlerFactory(_provider);
        var query1 = new TestGenericReadDbQueryDto<TestDto2>();
        var query2 = new TestGenericReadDbQueryEnt<TestDto2>();

        // Act
        var handler1 = factory.GetQueryHandler(query1, default(TestDto2)!);
        var handler2 = factory.GetQueryHandler(query2, default(TestDto2)!);

        // Assert
        using var _ = new AssertionScope();
        handler1.Should().BeOfType<TestGenericReadDbQueryDtoHandler<TestDto2>>();
        handler2.Should().BeOfType<TestGenericReadDbQueryEntHandler<TestDto2>>();
    }
}