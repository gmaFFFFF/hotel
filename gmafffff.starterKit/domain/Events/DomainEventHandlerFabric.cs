using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.Domain.Events;

public record DomainEventHandlerFabric(IServiceProvider ServiceProvider) : IDomainEventHandlerFabric {
    public IEnumerable<IDomainEventHandler> GetDomainEventHandlers(IDomainEvent @event) {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
        return ServiceProvider
            .GetServices(handlerType)
            .Cast<IDomainEventHandler>();
    }
}