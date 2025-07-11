using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class HotelBlocksRepository(HotelDbContext context, IDomainEventSink? domainEventSink = null) :
    Repository<HotelBlock<int, Guid>, int>(context,
        autoInclude: static hotel => hotel
            .Include(h => h.Tariffs)
            .Include(h => h.Rooms)
            .ThenInclude(r => r.RoomCleanings.AsQueryable().Where(cleaning => !cleaning.IsClean)),
        domainEventSink: domainEventSink),
    IHotelBlocksRepository<int, Guid> {
    protected Func<Expression<Func<Room<Guid>, bool>>, IQueryable<Room<Guid>>> GetRoom = null!;
    protected Func<Expression<Func<Room<Guid>, bool>>, IQueryable<HotelBlock<int, Guid>>> LoadWithRoomFilter = null!;

    public async Task<IImmutableList<HotelBlock<int, Guid>>> LoadWithRoomFilterAsync(
        Expression<Func<Room<Guid>, bool>> predicate, CancellationToken cancel = default) {
        return [.. await RunQueryAsync(LoadWithRoomFilter(predicate), cancel).ConfigureAwait(false)];
    }

    public async Task<int> CountRoomByAsync(Expression<Func<Room<Guid>, bool>> predicate,
        CancellationToken cancel = default) {
        return await GetRoom(predicate).CountAsync(cancellationToken: cancel);
    }

    public async Task<IList<Room<Guid>>> GetRoomsAsync(Expression<Func<Room<Guid>, bool>> predicate,
        Func<IQueryable<Room<Guid>>, IOrderedQueryable<Room<Guid>>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) {
        var query = QueryBuilder(
            GetRoom(predicate),
            sortOrder: sortOrder, pager: pager);
        return await RunQueryAsync(query, cancel).ConfigureAwait(false);
    }

    public async Task<IList<TRoomDto>> GetRoomsDtoAsync<TRoomDto>(
        Expression<Func<Room<Guid>, TRoomDto>> entityToDto,
        Expression<Func<Room<Guid>, bool>> predicate,
        Func<IQueryable<TRoomDto>, IOrderedQueryable<TRoomDto>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default)
        where TRoomDto : class {
        var query = QueryBuilder(GetRoom(predicate).Select(entityToDto), sortOrder: sortOrder, pager: pager);


        return await RunQueryAsync(query, cancel).ConfigureAwait(false);
    }

    protected override void DefineQuery() {
        base.DefineQuery();

        LoadWithRoomFilter = predicate
            => QueryBuilder(Entities,
                spec: h => h.Rooms.AsQueryable().Any(predicate),
                include: h => h
                    .Include(h => h.Rooms.AsQueryable().Where(predicate))
                    .ThenInclude(r => r.RoomCleanings.Where(cleaning => !cleaning.IsClean))
                    .Include(h => h.Tariffs)
            );

        GetRoom = predicate
            => QueryBuilder<Room<Guid>, int>(
                Context.Set<Room<Guid>>(),
                predicate,
                options: QueryTune.ChangeTrackingDisable
            );
    }
}