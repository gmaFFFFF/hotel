using AutoFixture.Xunit2;

namespace gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HotelInlineAutodataAttribute : InlineAutoDataAttribute {
    public HotelInlineAutodataAttribute(params object[] arguments) : base(new HotelAutodataAttribute(), arguments) { }
}