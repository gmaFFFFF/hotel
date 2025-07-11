using System.Collections.Immutable;

namespace gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;

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
    /// <param name="predicate">Условия отбора номеров</param>
    /// <param name="cancel">Токен отмены</param>
    Task<int> CountRoomByAsync(Expression<Func<Room<TPersonId>, bool>> predicate, CancellationToken cancel = default);

    /// <summary>
    ///     Возвращает номера в отеле
    /// </summary>
    /// <param name="predicate">Фильтр номеров</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Страничная выдача</param>
    /// <param name="cancel">Токен отмены</param>
    Task<IList<Room<TPersonId>>> GetRoomsAsync(Expression<Func<Room<TPersonId>, bool>> predicate,
        Func<IQueryable<Room<TPersonId>>, IOrderedQueryable<Room<TPersonId>>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default);

    /// <summary>
    ///     Возвращает DTO номера в отеле
    /// </summary>
    /// <param name="entityToDto">Проекция</param>
    /// <param name="predicate">Фильтр номеров</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Страничная выдача</param>
    /// <param name="cancel">Токен отмены</param>
    Task<IList<TRoomDto>> GetRoomsDtoAsync<TRoomDto>(
        Expression<Func<Room<Guid>, TRoomDto>> entityToDto,
        Expression<Func<Room<Guid>, bool>> predicate,
        Func<IQueryable<TRoomDto>, IOrderedQueryable<TRoomDto>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TRoomDto : class;

    #endregion
}