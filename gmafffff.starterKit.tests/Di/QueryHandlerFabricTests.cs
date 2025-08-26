using FluentAssertions.Execution;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Di;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.tests.Di.Fixtures;

namespace gmafffff.starterKit.tests.Di;

[TestSubject(typeof(QueryHandlerFabric))]
[TestSubject(typeof(RegisterServicesExtensions))]
public class QueryHandlerFabricTests {
    private readonly TestReadDbQueryDtoHandler _dtoHandler = new(null!, null!);
    private readonly TestReadDbQueryEntHandler _entHandler = new(null!, null!);
    private readonly IServiceProvider _provider;

    public QueryHandlerFabricTests() {
        _provider = Substitute.For<IServiceProvider>();
        _provider.GetService(typeof(IQueryHandler<TestReadDbQueryDto, TestDto>))
            .Returns(_dtoHandler);
        _provider.GetService(typeof(IQueryHandler<TestReadDbQueryEnt, TestDto>))
            .Returns(_entHandler);
        _provider.GetService(typeof(IQueryHandler<TestGenericReadDbQueryDto<TestDto2>, TestDto2>))
            .Returns(null);
        _provider.GetService(typeof(IQueryHandler<TestGenericReadDbQueryEnt<TestDto2>, TestDto2>))
            .Returns(null);
        _provider.GetService(typeof(IRepository<TestEntity, int>))
            .Returns(Substitute.For<IRepository<TestEntity, int>>());
        _provider.GetService(typeof(IEntityMapperForwardExpression<TestEntity, int, TestDto2>))
            .Returns(Substitute.For<IEntityMapperForwardExpression<TestEntity, int, TestDto2>>());
    }

    [Fact]
    public void Fabric_Get_NotGenericHandlers() {
        // Arrange
        var fabric = new QueryHandlerFabric(_provider);
        var query1 = new TestReadDbQueryDto();
        var query2 = new TestReadDbQueryEnt();

        // Act
        var handler1 = fabric.GetQueryHandler(query1, default(TestDto)!);
        var handler2 = fabric.GetQueryHandler(query2, default(TestDto)!);

        // Assert
        using var _ = new AssertionScope();
        handler1.Should().Be(_dtoHandler);
        handler2.Should().Be(_entHandler);
    }

    [Fact]
    public void Fabric_Add_GenericHandlers() {
        // Arrange, Act
        QueryHandlerFabric.TryAddGenericQueryHandler(typeof(TestGenericReadDbQueryDtoHandler<>));
        QueryHandlerFabric.TryAddGenericQueryHandler(typeof(TestGenericReadDbQueryEntHandler<>));

        // Assert
        using var _ = new AssertionScope();
        QueryHandlerFabric.GenericHandlers.Should().HaveCount(2);
        QueryHandlerFabric.GenericHandlers.Should().ContainKey(typeof(TestGenericReadDbQueryDto<>))
            .WhoseValue.Should().Be(typeof(TestGenericReadDbQueryDtoHandler<>));
        QueryHandlerFabric.GenericHandlers.Should().ContainKey(typeof(TestGenericReadDbQueryEnt<>))
            .WhoseValue.Should().Be(typeof(TestGenericReadDbQueryEntHandler<>));
    }


    [Fact]
    public void Fabric_Get_GenericHandlers() {
        // Arrange
        QueryHandlerFabric.TryAddGenericQueryHandler(typeof(TestGenericReadDbQueryDtoHandler<>));
        QueryHandlerFabric.TryAddGenericQueryHandler(typeof(TestGenericReadDbQueryEntHandler<>));
        var fabric = new QueryHandlerFabric(_provider);
        var query1 = new TestGenericReadDbQueryDto<TestDto2>();
        var query2 = new TestGenericReadDbQueryEnt<TestDto2>();

        // Act
        var handler1 = fabric.GetQueryHandler(query1, default(TestDto2)!);
        var handler2 = fabric.GetQueryHandler(query2, default(TestDto2)!);

        // Assert
        using var _ = new AssertionScope();
        handler1.Should().BeOfType<TestGenericReadDbQueryDtoHandler<TestDto2>>();
        handler2.Should().BeOfType<TestGenericReadDbQueryEntHandler<TestDto2>>();
    }
}