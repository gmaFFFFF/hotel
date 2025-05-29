using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.tests.Di.Fixtures;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public record TestDomainEvent(TestEntity Sender) : DomainEvent<TestEntity>(Sender);