namespace gmafffff.training.hotel.domain.tests.FakeModel.AutoFixture;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HotelInlineAutodataAttribute : InlineAutoDataAttribute {
    public HotelInlineAutodataAttribute(params object[] arguments) : base(new HotelAutodataAttribute(), arguments) { }
}