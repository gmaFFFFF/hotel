namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     Посещение номера
/// </summary>
public class RoomVisit<TPersonId> where TPersonId : struct, IEquatable<TPersonId> {
    /// <summary>
    ///     Посетители
    /// </summary>
    public ICollection<TPersonId> Visitors { get; set; } = null!;

    /// <summary>
    ///     Дата заезда
    /// </summary>
    public DateOnly ArrivalDate { get; set; }

    /// <summary>
    ///     Планируемая дата выезда
    /// </summary>
    public DateOnly? DepartureDatePlanned { get; set; }

    /// <summary>
    ///     Тариф на проживание
    /// </summary>
    public TariffDetails TariffDetails { get; set; }
}