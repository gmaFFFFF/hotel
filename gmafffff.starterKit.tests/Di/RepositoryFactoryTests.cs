using FluentAssertions.Execution;
using gmafffff.starterKit.Di;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.starterKit.tests.Di.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.tests.Di;

[TestSubject(typeof(RepositoryFactory<,,,>))]
[TestSubject(typeof(RegisterServicesExtensions))]
public class RepositoryFactoryTests {
    private readonly DbContext _fakeDbContext;
    private readonly IServiceProvider _provider;
    private readonly Service _service;

    public RepositoryFactoryTests() {
        _service = new Service();
        _fakeDbContext = Substitute.For<DbContext>(new DbContextOptions<DbContext>());

        var fakeContextFactory = Substitute.For<IDbContextFactory<DbContext>>();
        fakeContextFactory.CreateDbContext().Returns(_fakeDbContext);

        _provider = Substitute.For<IServiceProvider>();
        _provider.GetService<DbContext>().Returns(_fakeDbContext);
        _provider.GetService<IDbContextFactory<DbContext>>().Returns(fakeContextFactory);
        _provider.GetService<Service>().Returns(_service);
    }

    [Fact]
    public void Factory_Create_Repository() {
        // Arrange
        var factory = new RepositoryFactory<ITestRepository, TestRepository, TestEntity, int>(_provider);

        // Act
        var repo = factory.CreateTransient();

        // Assert
        using var _ = new AssertionScope();
        repo.Should().BeOfType<TestRepository>()
            .Which.FakeContext.Should().Be(_fakeDbContext);
        repo.Should().BeOfType<TestRepository>()
            .Which.Service.Should().Be(_service);
    }
}