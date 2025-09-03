using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Mappers;
using Microsoft.Extensions.Logging;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestReadDbQueryDtoHandler(
    IRepositoryReadOnly<TestEntity, int> repository,
    IEntityMapperForwardExpression<TestEntity, int, TestDto> mapper,
    ILogger<IQueryHandler<TestReadDbQueryDto, TestDto>>? logger = null)
    : ReadDbQueryHandler<TestReadDbQueryDto, TestEntity, int, IRepositoryReadOnly<TestEntity, int>,
                         TestDto>(repository, mapper, logger);

public class TestReadDbQueryEntHandler(
    IRepositoryReadOnly<TestEntity, int> repository,
    IEntityMapperForwardExpression<TestEntity, int, TestDto> mapper,
    ILogger<IQueryHandler<TestReadDbQueryEnt, TestDto>>? logger = null)
    : ReadDbQueryHandler<TestReadDbQueryEnt, TestEntity, int, IRepositoryReadOnly<TestEntity, int>,
                         TestDto>(repository, mapper, logger);

public class TestGenericReadDbQueryDtoHandler<TDto>(
    IRepositoryReadOnly<TestEntity, int> repository,
    IEntityMapperForwardExpression<TestEntity, int, TDto> mapper,
    ILogger<IQueryHandler<TestGenericReadDbQueryDto<TDto>, TDto>>? logger = null)
    : ReadDbQueryHandler<TestGenericReadDbQueryDto<TDto>, TestEntity, int, IRepositoryReadOnly<TestEntity, int>,
                         TDto>(repository, mapper, logger)
    where TDto : class;

public class TestGenericReadDbQueryEntHandler<TDto>(
    IRepositoryReadOnly<TestEntity, int> repository,
    IEntityMapperForwardExpression<TestEntity, int, TDto> mapper,
    ILogger<IQueryHandler<TestGenericReadDbQueryEnt<TDto>, TDto>>? logger = null)
    : ReadDbQueryHandler<TestGenericReadDbQueryEnt<TDto>, TestEntity, int, IRepositoryReadOnly<TestEntity, int>,
                         TDto>(repository, mapper, logger)
    where TDto : class;