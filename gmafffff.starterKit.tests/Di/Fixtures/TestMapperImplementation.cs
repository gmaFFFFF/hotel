using System.Linq.Expressions;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestMapperImplementation : ITestMapperMapster {
    public Expression<Func<TestEntity, TestDto>> EntityToDto { get; set; }

    public TestDto? Map(TestEntity? entity) {
        throw new NotImplementedException();
    }

    public TestEntity? Map(TestDto? dto) {
        throw new NotImplementedException();
    }

    public TestEntity? Update(TestDto? sourceDto, TestEntity? targetEntity) {
        throw new NotImplementedException();
    }

    public TestDto? Update(TestEntity? sourceEntity, TestDto? targetDto) {
        throw new NotImplementedException();
    }
}