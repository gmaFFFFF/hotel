using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     Номер отеля
/// </summary>
public class Room<TPersonId> : Entity<int>
    where TPersonId : struct, IEquatable<TPersonId> {
    /// <summary>
    ///     Описание
    /// </summary>
    public RoomDetails RoomDetails { get; set; }

    /// <summary>
    ///     Посещение номера
    /// </summary>
    public RoomVisit<TPersonId>? Visit { get; set; }

    /// <summary>
    ///     Свободен ли номер
    /// </summary>
    public bool IsFree => Visit is null;

    /// <summary>
    ///     Сегодня планируется ли выселение
    /// </summary>
    public bool? IsFreeToday
        => Visit?.DepartureDatePlanned?.DayNumber <= DateOnly.FromDateTime(DateTime.Today).DayNumber;

    # region Базовые фильтры

    /// <summary>
    ///     Только свободные номера
    /// </summary>
    public static readonly Expression<Func<Room<TPersonId>, bool>> IsFreeRoom = static room => room.Visit == null;

    /// <summary>
    ///     Только помещение (гостиничный номер) с определённым номером
    /// </summary>
    /// <param name="number">номер помещения (гостиничного номера)</param>
    /// <returns></returns>
    public static Expression<Func<Room<TPersonId>, bool>> FilterByNumber(string number) {
        return room => room.RoomDetails.Number == number;
    }

    /// <summary>
    ///     Только помещения (гостиничные номера) определённой категории
    /// </summary>
    /// <param name="type">категория помещения (гостиничного номера)</param>
    /// <returns></returns>
    public static Expression<Func<Room<TPersonId>, bool>> FilterByType(RoomType type) {
        return room => room.RoomDetails.Type == type;
    }

    #endregion
}