using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public record TestDomainEvent(int Num): DomainEvent<Entity<int>>((Entity<int>?) null!);