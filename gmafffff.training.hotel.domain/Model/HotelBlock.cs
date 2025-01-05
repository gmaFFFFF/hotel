namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     Блок (здание, корпус) гостиницы
/// </summary>
public class HotelBlock<TId, TPersonId> : Entity<TId>
    where TId : struct, IEquatable<TId>
    where TPersonId : struct, IEquatable<TPersonId> {
    public string? Name { get; set; }

    /// <summary>
    ///     Доступный номерной фонд
    /// </summary>
    public ICollection<Room<TPersonId>> Rooms { get; set; } = null!;

    /// <summary>
    ///     Тарифы
    /// </summary>
    public ICollection<Tariff> Tariffs { get; set; } = null!;

    /// <summary>
    ///     Найди подходящие свободные номера по критериям
    /// </summary>
    public IEnumerable<Room<TPersonId>> FindSuitable(RoomType type, int capacity) {
        return Rooms
            .Where(room => room.IsFree)
            .Where(room => room.RoomDetails.Type >= type && room.RoomDetails.Capacity >= capacity);
    }

    /// <summary>
    ///     Засели посетителя
    /// </summary>
    public RoomVisit<TPersonId> SettledIn(IEnumerable<TPersonId> persons, Room<TPersonId> room,
        DateOnly? arrivalDate = null, DateOnly? departureDatePlanned = null) {
        var newVisit = () => new RoomVisit<TPersonId> {
            Visitors = [..persons],
            ArrivalDate = arrivalDate ?? DateOnly.FromDateTime(DateTime.Today),
            DepartureDatePlanned = departureDatePlanned,
            TariffDetails = Tariffs
                .Single(t => t.TariffDetails.Type == room.RoomDetails.Type)
                .TariffDetails with { }
        };

        room.Visit ??= newVisit();
        room.Visit.Visitors = room.Visit.Visitors.Concat(persons).Distinct().ToList();

        return room.Visit;
    }

    /// <summary>
    ///     Прими номер
    /// </summary>
    public AccommodationReport<TPersonId>? MoveOut(Room<TPersonId> room, DateOnly? departureDate) {
        if (room.IsFree) return null;

        var report = new AccommodationReport<TPersonId> {
            RoomDetails = room.RoomDetails,
            TariffDetails = room.Visit.TariffDetails with { },
            Visitors = room.Visit.Visitors,
            ArrivalDate = room.Visit.ArrivalDate,
            DepartureDate = departureDate ?? DateOnly.FromDateTime(DateTime.Today)
        };

        room.Visit = null;

        return report;
    }
}