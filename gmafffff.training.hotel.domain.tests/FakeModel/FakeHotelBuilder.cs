namespace gmafffff.training.hotel.domain.tests.FakeModel;

public class FakeHotelBuilder {
    private readonly IFixture _fixture = new Fixture().Customize(new FakeHotelCustomization());

    /// <summary>
    ///     Ежедневный процент потенциальных клиентов от числа номеров в отеле по месяцам
    /// </summary>
    private readonly int[] _interest = [1, 1, 2, 3, 5, 8, 13, 8, 5, 3, 2, 1];

    /// <summary>
    ///     Гости отеля
    /// </summary>
    private readonly HashSet<Person<Guid>> _persons = new();

    /// <summary>
    ///     Отчеты о проживании
    /// </summary>
    private readonly List<AccommodationReport<Guid>> _reports = [];

    private readonly Random _rnd = new();

    /// <summary>
    ///     Конец моделирования
    /// </summary>
    private DateOnly _endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

    /// <summary>
    ///     Фейковый отель
    /// </summary>
    private HotelBlock<int, Guid> _hotel;

    /// <summary>
    ///     Начало моделирования
    /// </summary>
    private DateOnly _startDate;

    /// <summary>
    ///     Количество номеров в <see cref="_hotel" />
    /// </summary>
    private int RoomsCount => _hotel.Rooms.Count;

    public FakeHotel Build(DateOnly startDate = default, ushort roomCount = 21) {
        FakeHotelCustomization.RoomsCount = roomCount;

        if (startDate == default) {
            var randomDay = _rnd.Next(_endDate.AddYears(-2).DayNumber, _endDate.DayNumber - 1);
            startDate = DateOnly.FromDayNumber(randomDay);
        }

        _startDate = startDate;

        if (_startDate.DayNumber > _endDate.DayNumber)
            (_startDate, _endDate) = (_endDate, _startDate);

        _hotel = _fixture.Create<HotelBlock<int, Guid>>();

        StartSimulation();

        return new FakeHotel(_hotel, _reports, _persons);
    }

    private void StartSimulation() {
        for (var i = _startDate.DayNumber; i <= _endDate.DayNumber; i++) {
            var day = DateOnly.FromDayNumber(i);
            MoveOut(day);
            var potentialOrders = GetPotentialOrders(day);
            SettledIn(potentialOrders, day);
        }
    }

    private void MoveOut(DateOnly day) {
        foreach (var room in _hotel.Rooms.Where(r => !r.IsFree)) {
            if (room.Visit!.DepartureDatePlanned?.DayNumber >= day.DayNumber) continue;

            var report = _hotel.MoveOut(room, day);
            _reports.Add(report!);
        }
    }

    private void SettledIn(IEnumerable<(Person<Guid> person, RoomType roomType, int duration)> orders,
        DateOnly day) {
        foreach (var (person, roomType, duration) in orders) {
            // номер, в который уже заселен член семьи
            var occupiedRoom = (from room in _hotel.Rooms
                    where room.Visit is not null
                    from personId in room.Visit.Visitors
                    join personInfo in _persons on personId equals personInfo.Id
                    select (personInfo.FullName.SurName, room)
                    into visitor
                    where string.Equals(visitor.SurName, person.FullName.SurName, StringComparison.OrdinalIgnoreCase)
                    select visitor.room)
                .FirstOrDefault();

            // свободный подходящий номер
            var free = _hotel.FindSuitable(roomType, capacity: 1)
                .MinBy(r => r.RoomDetails.Type);

            var findRoom = occupiedRoom ?? free;
            if (findRoom is null) continue;

            _hotel.SettledIn(new[] { person.Id }, findRoom, day, day.AddDays(duration));

            _persons.Add(person);
        }
    }

    private IEnumerable<(Person<Guid> person, RoomType roomType, int duration)> GetPotentialOrders(DateOnly day) {
        var count = Convert.ToInt32(_interest[day.Month - 1] * .01 * RoomsCount);
        if (count == 0) count = 1;

        var roomType = _fixture.CreateMany<RoomType>(count);
        var durations = Enumerable.Repeat(element: () => _rnd.Next(minValue: 1, maxValue: 14), count)
            .Select(run => run());
        var persons = _fixture.CreateMany<Person<Guid>>(count)
            .Select(p => _persons.SingleOrDefault(o => o.FullName == p.FullName) ?? p);
        return persons.Zip(roomType, durations);
    }
}