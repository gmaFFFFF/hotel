namespace gmafffff.training.hotel.domain.tests.FakeModel.AutoFixture;

public class RoomDetailsGenerator : ISpecimenBuilder {
    private readonly IEnumerator<int> _intEnumerator = Enumerable.Range(start: 1, int.MaxValue).GetEnumerator();

    private readonly Random _random = new();

    public object Create(object request, ISpecimenContext context) {
        return request switch {
            Type t when t == typeof(RoomDetails) => new RoomDetails(
                GetNextNumber(),
                context.Create<RoomType>(),
                (byte)_random.Next(minValue: 2, maxValue: 6)
            ),
            _ => new NoSpecimen()
        };
    }

    private string GetNextNumber() {
        _intEnumerator.MoveNext();
        return _intEnumerator.Current.ToString();
    }
}