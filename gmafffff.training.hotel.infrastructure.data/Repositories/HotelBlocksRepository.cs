using gmafffff.starterKit.Db;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class HotelBlocksRepository :
    RepositoryEfCore<HotelBlock<int, Guid>, int>,
    IHotelBlocksRepository<int, Guid> {
    protected readonly IPropertyManagementMapper PropertyManagementMapper;


    protected Func<Expression<Func<Room<Guid>, bool>>, IQueryable<Room<Guid>>> GetRoom = null!;
    protected Func<Expression<Func<Room<Guid>, bool>>, IQueryable<HotelBlock<int, Guid>>> LoadWithRoomFilter = null!;

    public HotelBlocksRepository(HotelDbContext context, IPropertyManagementMapper propertyManagementMapper) : base(
        context,
        autoInclude: static hotel => hotel
            .Include(h => h.Tariffs)
            .Include(h => h.Rooms)) {
        PropertyManagementMapper = propertyManagementMapper;
    }

    public async Task<IImmutableList<HotelBlock<int, Guid>>> LoadWithRoomFilterAsync(
        Expression<Func<Room<Guid>, bool>> predicate, CancellationToken cancel = default) {
        return await RunQuery(LoadWithRoomFilter(predicate), cancel).ConfigureAwait(false);
    }

    public async Task<int> CountRoomByAsync(Expression<Func<Room<Guid>, bool>> predicate,
        CancellationToken cancel = default) {
        return await GetRoom(predicate).CountAsync(cancellationToken: cancel);
    }

    public async Task<IImmutableList<RoomDto>> GetRoomsAsync(Expression<Func<Room<Guid>, bool>> predicate,
        (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) {
        ArgumentNullException.ThrowIfNull(PropertyManagementMapper);
        var query = QueryBuilder<Room<Guid>, int>(GetRoom(predicate), pager: pager);
        return await RunQuery(query.Select(PropertyManagementMapper.EntityToDto), cancel)
            .ConfigureAwait(false);
    }

    protected override void DefineQuery() {
        base.DefineQuery();

        LoadWithRoomFilter = predicate
            => QueryBuilder(Entities,
                spec: h => h.Rooms.AsQueryable().Any(predicate),
                include: h => h
                    .Include(h => h.Rooms.AsQueryable().Where(predicate))
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