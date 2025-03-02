using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.tests.Domain.Fixtures;

internal class TestEntityWithCustomId(MyId id) : Entity<MyId>(id);