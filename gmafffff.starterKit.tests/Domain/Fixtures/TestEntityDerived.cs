namespace gmafffff.starterKit.tests.Domain.Fixtures;

internal class TestEntityDerived : TestEntity {
    public TestEntityDerived(string property) : base(property) { }
    public TestEntityDerived(int id) : base(id) { }
}