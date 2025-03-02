using AutoFixture.Xunit2;

namespace gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;

[AttributeUsage(AttributeTargets.Method)]
public class HotelAutodataAttribute : AutoDataAttribute {
    public HotelAutodataAttribute() : base(CreateFixture) { }

    public static IFixture CreateFixture() {
        return new Fixture().Customize(new FakeHotelCustomization());
    }
}