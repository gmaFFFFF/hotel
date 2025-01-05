namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     Отчет о проживании
/// </summary>
public class AccommodationReport<TPersonId> : Entity<int>
    where TPersonId : struct, IEquatable<TPersonId> {
    /// <summary>
    ///     Информация о номере
    /// </summary>
    public RoomDetails RoomDetails { get; set; }

    /// <summary>
    ///     Информация о тарифе
    /// </summary>
    public TariffDetails TariffDetails { get; set; }

    /// <summary>
    ///     Жильцы
    /// </summary>
    public ICollection<TPersonId> Visitors { get; set; } = null!;

    /// <summary>
    ///     Дата заезда
    /// </summary>
    public DateOnly ArrivalDate { get; set; }

    /// <summary>
    ///     Дата выезда
    /// </summary>
    public DateOnly DepartureDate { get; set; }

    /// <summary>
    ///     Длительность проживания
    /// </summary>
    public decimal Duration => DepartureDate.DayNumber - ArrivalDate.DayNumber;

    /// <summary>
    ///     Стоимость проживания
    /// </summary>
    public decimal Price => TariffDetails.Value * Duration;
}