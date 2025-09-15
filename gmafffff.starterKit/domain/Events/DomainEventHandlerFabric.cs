using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.Domain.Events;

public record DomainEventHandlerFactory(IServiceProvider ServiceProvider) {
    public virtual IEnumerable<IDomainEventHandler> GetDomainEventHandlers(IDomainEvent @event) {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
        return ServiceProvider
            .GetServices(handlerType)
            .Cast<IDomainEventHandler>();
    }
}