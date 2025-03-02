using AutoFixture;
using AutoFixture.Idioms;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.tests.Domain.Fixtures;

namespace gmafffff.starterKit.tests.Domain;

public class EntityTests {
    private readonly Fixture _fixture = new();

    /// <summary>
    ///     Реализовано сравнение на равенство
    /// </summary>
    [Fact]
    public void ComparisonForEqualityImplemented() {
        _fixture
            .Create<EqualityComparerAssertion>()
            .Verify(typeof(Entity<>));
    }

    /// <summary>
    ///     Производная сущность не равна базовой
    /// </summary>
    [Fact]
    public void DerivedEntityNotEqualBase() {
        // Arrange
        var ent1 = new TestEntity(1);
        var ent2 = new TestEntityDerived(1);

        // Act, Assert
        ent1.Equals(ent2).Should().BeFalse();
        (ent1 == ent2).Should().BeFalse();
    }

    /// <summary>
    ///     Сущности с равными идентификаторами равны
    /// </summary>
    [Fact]
    public void EntitiesWithEqualIdentifiersEqual() {
        // Arrange
        var ent1 = new TestEntity(1);
        var ent2 = new TestEntity(1);

        // Act, Assert
        ent1.Equals(ent2).Should().BeTrue();
        (ent1 == ent2).Should().BeTrue();
    }

    /// <summary>
    ///     Сущности с разными идентификаторами не равны
    /// </summary>
    [Fact]
    public void EntitiesWithDifferentIdentifiersNotEqual() {
        // Arrange
        var ent1 = new TestEntity(1);
        var ent2 = new TestEntity(2);

        // Act, Assert
        ent1.Equals(ent2).Should().BeFalse();
        (ent1 == ent2).Should().BeFalse();
    }

    /// <summary>
    ///     Сущности с default(id) не равны
    /// </summary>
    [Fact]
    public void EntitiesWithDefaultIdentifiersNotEqual() {
        // Arrange
        var ent1 = new TestEntity(default(int));
        var ent2 = new TestEntity(default(int));

        // Act, Assert
        ent1.Equals(ent2).Should().BeFalse();
        (ent1 == ent2).Should().BeFalse();
    }

    /// <summary>
    ///     Поддерживаются сущности с комплексным идентификатором
    /// </summary>
    [Fact]
    public void EntitiesWithComprehensiveIdentifierSupported() {
        // Arrange
        var guid = new Guid();
        var ent1 = new TestEntityWithCustomId(new MyId("1", guid));
        var ent2 = new TestEntityWithCustomId(new MyId("1", guid));

        // Act, Assert
        ent1.Equals(ent2).Should().BeTrue();
        (ent1 == ent2).Should().BeTrue();
    }

    /// <summary>
    ///     Корректное сравнение с null
    /// </summary>
    [Fact]
    public void CorrectComparisonWithNull() {
        // Arrange
        TestEntity? ent1 = new(1);
        TestEntity? nullEnt1 = null;
        TestEntity? nullEnt2 = null;

        // Act, Assert
        (ent1 == null).Should().BeFalse();
        (nullEnt1 == null).Should().BeTrue();
        ent1?.Equals(null).Should().BeFalse();
        (nullEnt1 == nullEnt2).Should().BeTrue();
    }

    /// <summary>
    ///     Сущности с default комплексным идентификатором равным null не равны
    /// </summary>
    [Fact]
    public void EntitiesWithDefaultComprehensiveIdentifierNotEqual() {
        var ent1 = new TestEntityWithCustomId(default);
        var ent2 = new TestEntityWithCustomId(default);

        (ent1 == ent2).Should().BeFalse();
    }
}