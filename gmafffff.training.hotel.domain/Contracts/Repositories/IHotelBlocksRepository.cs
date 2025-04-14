using System.Collections.Immutable;
using gmafffff.starterKit.Domain;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;

namespace gmafffff.training.hotel.domain.Contracts.Repositories;

public interface IHotelBlocksRepository<TId, TPersonId> : IRepository<HotelBlock<TId, TPersonId>, TId>
    where TId : struct, IEquatable<TId>
    where TPersonId : struct, IEquatable<TPersonId> {
    # region Запросы

    /// <summary>
    ///     Фильтрует гостиничные номера в агрегате
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancel"></param>
    Task<IImmutableList<HotelBlock<int, Guid>>> LoadWithRoomFilterAsync(
        Expression<Func<Room<TPersonId>, bool>> predicate, CancellationToken cancel = default);

    /// <summary>
    ///     Возвращает количество номеров, соответствующих <paramref name="predicate" />
    /// </summary>
    /// <param name="predicate">условия отбора номеров</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    Task<int> CountRoomByAsync(Expression<Func<Room<TPersonId>, bool>> predicate, CancellationToken cancel = default);

    /// <summary>
    ///     Возвращает DTO номеров в отеле
    /// </summary>
    /// <param name="predicate">фильтр номеров</param>
    /// <param name="sortOrder">порядок сортировки</param>
    /// <param name="pager">страничная выдача</param>
    /// <param name="cancel"></param>
    Task<IList<RoomDto>> GetRoomsAsync(Expression<Func<Room<TPersonId>, bool>> predicate,
        Func<IQueryable<RoomDto>, IOrderedQueryable<RoomDto>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default);

    #endregion
}