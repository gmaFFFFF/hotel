using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;

namespace gmafffff.starterKit.tests.Domain.EntityFrameworkCore.Fixtures;

internal class SimpleRepo(SimpleDbContext dbContext, IDomainEventSink eventSink)
    : Repository<SimpleEntity, int>(dbContext, eventSink);