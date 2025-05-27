using gmafffff.starterKit.Domain.Events;

namespace gmafffff.starterKit.tests.Domain.EntityFrameworkCore.Fixtures;

internal record SimpleDomainEvent(SimpleEntity Sender) : DomainEvent<SimpleEntity>(Sender);

internal record SimpleDomainEvent2(SimpleEntity Sender) : DomainEvent<SimpleEntity>(Sender);

internal record SimpleDomainEvent3(SimpleEntity Sender) : DomainEvent<SimpleEntity>(Sender);