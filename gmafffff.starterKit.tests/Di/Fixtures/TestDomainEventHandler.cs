using gmafffff.starterKit.Domain.Events;

namespace gmafffff.starterKit.Tests.Di.Fixtures;

public class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent> {
    public Task<Fin<Unit>> HandleAsync(TestDomainEvent @event, DomainEventDispatcherContext context, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }
}