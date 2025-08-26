using gmafffff.starterKit.Messaging.Crud;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public record TestReadDbQueryDto() : ReadDbQuery<TestDto>(x => true);

public record TestReadDbQueryEnt() : ReadDbQuery<TestEntity, TestDto>(x => true);

public record TestGenericReadDbQueryDto<TDto>() : ReadDbQuery<TDto>(x => true);

public record TestGenericReadDbQueryEnt<TDto>() : ReadDbQuery<TestEntity, TDto>(x => true);