using gmafffff.starterKit.Domain.Events;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public record TestDomainEvent(TestEntity Sender) : DomainEvent<TestEntity>(Sender);