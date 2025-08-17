using gmafffff.starterKit.Utils;

namespace gmafffff.training.hotel.domain.PropertyManagement.Models;

public partial class Room<TPersonId>
    where TPersonId : struct, IEquatable<TPersonId> {
    #region Базовые фильтры

    /// <summary>
    ///     Только свободные номера
    /// </summary>
    public static readonly Expression<Func<Room<TPersonId>, bool>> WhereFreeRoom = static room => room.Visit == null;

    /// <summary>
    ///     Только чистые номера
    /// </summary>
    public static readonly Expression<Func<Room<TPersonId>, bool>> WhereCleanRoom = static room
        => room.RoomCleanings.All(cleaning => cleaning.IsClean);

    /// <summary>
    ///     Только свободные и чистые номера
    /// </summary>
    public static readonly Expression<Func<Room<TPersonId>, bool>> WhereFreeAndCleanRoom =
        WhereFreeRoom.And(WhereCleanRoom);

    /// <summary>
    ///     Только помещение (гостиничный номер) с определённым номером
    /// </summary>
    /// <param name="number">Номер помещения (гостиничного номера)</param>
    /// <returns></returns>
    public static Expression<Func<Room<TPersonId>, bool>> WhereNumber(string number) {
        return room => room.RoomDetails.Number == number;
    }

    /// <summary>
    ///     Только помещения (гостиничные номера) определённой категории
    /// </summary>
    /// <param name="type">Категория помещения (гостиничного номера)</param>
    /// <returns></returns>
    public static Expression<Func<Room<TPersonId>, bool>> WhereType(RoomType type) {
        return room => room.RoomDetails.Type == type;
    }

    #endregion
}