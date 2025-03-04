using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public interface ITestRepositoryFactory : IRepositoryFactory<ITestRepository, TestEntity, int>;