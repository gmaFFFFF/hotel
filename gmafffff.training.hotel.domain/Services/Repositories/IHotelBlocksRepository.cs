using System.Collections.Immutable;

namespace gmafffff.training.hotel.domain.Services.Repositories;

public interface IHotelBlocksRepository<TId, TPersonId> : IRepository<HotelBlock<TId, TPersonId>, TId>
    where TId : struct, IEquatable<TId>
    where TPersonId : struct, IEquatable<TPersonId> {
    # region Запросы

    /// <summary>
    ///     Фильтрует гостиничные номера в агрегате
    /// </summary>
    /// <param name="predicate"></param>
    Task<IImmutableList<HotelBlock<int, Guid>>> LoadWithRoomFilterAsync(
        Expression<Func<Room<TPersonId>, bool>> predicate);

    /// <summary>
    ///     Возвращает количество номеров, соответствующих <paramref name="predicate" />
    /// </summary>
    /// <param name="predicate">условия отбора номеров</param>
    /// <returns></returns>
    Task<int> CountRoomByAsync(Expression<Func<Room<TPersonId>, bool>> predicate);

    #endregion
}