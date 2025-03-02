using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.tests.Domain.Fixtures;

internal class TestEntity : Entity<int> {
    public TestEntity(string property) {
        Property = property;
    }

    public TestEntity(int id) : base(id) { }
    public string? Property { get; }
}