using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using Microsoft.Extensions.Logging;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestReadDbQueryDtoHandler(
    IRepository<TestEntity, int> repository,
    IEntityMapperForwardExpression<TestEntity, int, TestDto> mapper,
    ILogger<IQueryHandler<TestReadDbQueryDto, TestDto>>? logger = null)
    : ReadDbQueryHandler<TestReadDbQueryDto, TestEntity, int, TestDto>(repository, mapper, logger);

public class TestReadDbQueryEntHandler(
    IRepository<TestEntity, int> repository,
    IEntityMapperForwardExpression<TestEntity, int, TestDto> mapper,
    ILogger<IQueryHandler<TestReadDbQueryEnt, TestDto>>? logger = null)
    : ReadDbQueryHandler<TestReadDbQueryEnt, TestEntity, int, TestDto>(repository, mapper, logger);

public class TestGenericReadDbQueryDtoHandler<TDto>(
    IRepository<TestEntity, int> repository,
    IEntityMapperForwardExpression<TestEntity, int, TDto> mapper,
    ILogger<IQueryHandler<TestGenericReadDbQueryDto<TDto>, TDto>>? logger = null)
    : ReadDbQueryHandler<TestGenericReadDbQueryDto<TDto>, TestEntity, int, TDto>(repository, mapper, logger)
    where TDto : class;

public class TestGenericReadDbQueryEntHandler<TDto>(
    IRepository<TestEntity, int> repository,
    IEntityMapperForwardExpression<TestEntity, int, TDto> mapper,
    ILogger<IQueryHandler<TestGenericReadDbQueryEnt<TDto>, TDto>>? logger = null)
    : ReadDbQueryHandler<TestGenericReadDbQueryEnt<TDto>, TestEntity, int, TDto>(repository, mapper, logger)
    where TDto : class;