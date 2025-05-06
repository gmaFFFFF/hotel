using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Di;
using gmafffff.starterKit.Mappers;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.tests.Di.Fixtures;
using gmafffff.starterKit.tests.Validation.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Validot;

namespace gmafffff.starterKit.tests.Di;

[TestSubject(typeof(RegisterServicesExtensions))]
public class RegisterServicesTests {
    /// <summary>
    ///     Регистрирует бизнес-правила
    /// </summary>
    [Fact]
    public void RegisterBusinessRules() {
        // Arrange
        var provider = Substitute.For<IServiceCollection>();

        // Act
        provider.AddBusinessRules(typeof(RegisterServicesTests).Assembly);

        // Assert
        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(IBusinessRule<BusinessCommand>) &&
            descriptor.ImplementationType == typeof(TestBusinessRule) &&
            descriptor.Lifetime == ServiceLifetime.Scoped));
    }

    /// <summary>
    ///     Регистрирует проверяющих
    /// </summary>
    [Fact]
    public void RegisterValidators() {
        // Arrange
        var provider = Substitute.For<IServiceCollection>();

        // Act
        provider.AddValidotValidators(typeof(RegisterServicesTests).Assembly);

        // Assert
        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(IValidator<ValidableClass>) &&
            descriptor.ImplementationInstance.GetType() == typeof(Validator<ValidableClass>) &&
            descriptor.Lifetime == ServiceLifetime.Singleton));
    }

    /// <summary>
    ///     Регистрирует преобразователи
    /// </summary>
    [Fact]
    public void RegisterMappers() {
        // Arrange
        var provider = Substitute.For<IServiceCollection>();

        // Act
        provider.AddEntityMappers(typeof(RegisterServicesTests).Assembly);

        // Assert
        provider.Received(3);
        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(TestMapperImplementation) &&
            descriptor.ImplementationType == typeof(TestMapperImplementation) &&
            descriptor.Lifetime == ServiceLifetime.Singleton));

        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(ITestMapper) &&
            descriptor.ImplementationFactory != null &&
            descriptor.Lifetime == ServiceLifetime.Singleton));

        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(ITestMapperMapster) &&
            descriptor.ImplementationFactory != null &&
            descriptor.Lifetime == ServiceLifetime.Singleton));

        provider.DidNotReceive().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType.IsGenericType &&
            (descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IEntityMapper<,,>) ||
             descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IEntityMapperForward<,,>) ||
             descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IEntityMapperBackward<,,>) ||
             descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IEntityMapperDuplex<,,>) ||
             descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IEntityMapperForwardExpression<,,>)
            )));
    }

    /// <summary>
    ///     Регистрирует кладовые и фабрики
    /// </summary>
    [Fact]
    public void RegisterRepository() {
        // Arrange
        var provider = Substitute.For<IServiceCollection>();

        // Act
        provider.AddRepositories(typeof(RegisterServicesTests).Assembly);

        // Assert
        provider.Received(2);

        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(ITestRepository) &&
            descriptor.ImplementationType == typeof(TestRepository) &&
            descriptor.Lifetime == ServiceLifetime.Scoped));

        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType == typeof(ITestRepositoryFactory) &&
            descriptor.ImplementationType == typeof(TestRepositoryFactory) &&
            descriptor.Lifetime == ServiceLifetime.Singleton));
    }

    /// <summary>
    ///     Регистрирует обработчики команд
    /// </summary>
    [Fact]
    public void RegisterBusinessCommandDbHandler() {
        // Arrange
        var provider = Substitute.For<IServiceCollection>();

        // Act
        provider.AddBusinessCommandDbHandlers(typeof(TestBusinessCommandDbHandler).Assembly);

        // Assert
        provider.Received(1);

        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType.IsGenericType &&
            descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IBusinessCommandHandler<>) &&
            descriptor.ImplementationType == typeof(TestBusinessCommandDbHandler) &&
            descriptor.Lifetime == ServiceLifetime.Transient));
    }

    /// <summary>
    ///     Регистрирует обработчики запросов
    /// </summary>
    [Fact]
    public void RegisterQueryHandler() {
        // Arrange
        var provider = Substitute.For<IServiceCollection>();

        // Act
        provider.AddQueryHandlers(typeof(TestQueryHandler).Assembly);

        // Assert
        provider.Received(1);

        provider.Received().Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ServiceType.IsGenericType &&
            descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) &&
            descriptor.ImplementationType == typeof(TestQueryHandler) &&
            descriptor.Lifetime == ServiceLifetime.Transient));
    }
}