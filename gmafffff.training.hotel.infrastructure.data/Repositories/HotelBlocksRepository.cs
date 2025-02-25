using gmafffff.starterKit.Db;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class HotelBlocksRepository :
    RepositoryEfCore<HotelBlock<int, Guid>, int>,
    IHotelBlocksRepository<int, Guid> {
    protected readonly IPropertyManagementMapper PropertyManagementMapper;
    protected Func<Expression<Func<Room<Guid>, bool>>, IQueryable<Room<Guid>>> GetRoom;
    protected Func<Expression<Func<Room<Guid>, bool>>, IQueryable<HotelBlock<int, Guid>>> LoadWithRoomFilter;


    public HotelBlocksRepository(DbContext context, IPropertyManagementMapper propertyManagementMapper) : base(
        context,
        autoInclude: static hotel => hotel.Include(h => h.Tariffs).Include(h => h.Rooms)) {
        PropertyManagementMapper = propertyManagementMapper;
    }


    public async Task<IImmutableList<HotelBlock<int, Guid>>> LoadWithRoomFilterAsync(
        Expression<Func<Room<Guid>, bool>> predicate) {
        return await RunQuery(LoadWithRoomFilter(predicate)).ConfigureAwait(false);
    }

    public async Task<int> CountRoomByAsync(Expression<Func<Room<Guid>, bool>> predicate) {
        return await GetRoom(predicate).CountAsync();
    }

    public async Task<IImmutableList<RoomDto>> GetRoomsAsync(Expression<Func<Room<Guid>, bool>> predicate) {
        ArgumentNullException.ThrowIfNull(PropertyManagementMapper);

        return await RunQuery(GetRoom(predicate)
            .Select(PropertyManagementMapper.RoomToRoomDto)).ConfigureAwait(false);
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