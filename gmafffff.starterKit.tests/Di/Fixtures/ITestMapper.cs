using gmafffff.starterKit.Mappers;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public interface ITestMapper : IEntityMapperDuplex<TestEntity, int, TestDto>;