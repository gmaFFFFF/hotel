namespace gmafffff.training.hotel.domain.tests.FakeModel.AutoFixture;

[AttributeUsage(AttributeTargets.Method)]
public class HotelAutodataAttribute : AutoDataAttribute {
    public HotelAutodataAttribute() : base(CreateFixture) { }

    public static IFixture CreateFixture() {
        return new Fixture().Customize(new FakeHotelCustomization());
    }
}