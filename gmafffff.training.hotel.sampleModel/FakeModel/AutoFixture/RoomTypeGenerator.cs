namespace gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;

public class RoomTypeGenerator : ISpecimenBuilder {
    private readonly RoomType[] _roomTypes;
    private int _index;

    public RoomTypeGenerator() {
        IEnumerable<RoomType>[] allocationType = [
            Enumerable.Repeat(RoomType.None, count: 1), Enumerable.Repeat(RoomType.Lux, count: 2),
            Enumerable.Repeat(RoomType.SemiLux, count: 3), Enumerable.Repeat(RoomType.Standard, count: 34)
        ];
        _roomTypes = allocationType.SelectMany(x => x).ToArray();
        new Random().Shuffle(_roomTypes);
    }

    public object Create(object request, ISpecimenContext context) {
        return request switch {
            SeededRequest s when (Type)s.Request == typeof(RoomType) => GetNext(),
            Type t when t == typeof(RoomType) => GetNext(),
            _ => new NoSpecimen()
        };
    }

    private RoomType GetNext() {
        if (_index >= _roomTypes.Length)
            _index = 0;
        return _roomTypes[_index++];
    }
}