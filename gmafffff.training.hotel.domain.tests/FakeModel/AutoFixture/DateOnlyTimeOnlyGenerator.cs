namespace gmafffff.training.hotel.domain.tests.FakeModel.AutoFixture;

public class DateOnlyTimeOnlyGenerator : ISpecimenBuilder {
    public object Create(object request, ISpecimenContext context) {
        return request switch {
            Type type when type == typeof(DateOnly)
                => DateOnly.FromDateTime(context.Create<DateTime>()),
            Type type2 when type2 == typeof(TimeOnly)
                => TimeOnly.FromDateTime(context.Create<DateTime>()),
            _ => new NoSpecimen()
        };
    }
}