namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class HotelBlocksRepository :
    RepositoryEfCore<HotelBlock<int, Guid>, int>,
    IHotelBlocksRepository<int, Guid> {
    protected readonly IPropertyManagementMapper PropertyManagementMapper;
    protected Func<Expression<Func<Room<Guid>, bool>>, IQueryable<Room<Guid>>> GetRoom;
    protected Func<Expression<Func<Room<Guid>, bool>>, IQueryable<HotelBlock<int, Guid>>> LoadWithRoomFilter;


    public HotelBlocksRepository(DbContext context, IPropertyManagementMapper propertyManagementMapper) : base(
        context,
        static hotel => hotel.Include(h => h.Tariffs).Include(h => h.Rooms)) {
        PropertyManagementMapper = propertyManagementMapper;
    }


    public async Task<IImmutableList<HotelBlock<int, Guid>>> LoadWithRoomFilterAsync(
        Expression<Func<Room<Guid>, bool>> predicate) {
        return await RunQuery(LoadWithRoomFilter(predicate)).ConfigureAwait(false);
    }

    public async Task<int> CountRoomByAsync(Expression<Func<Room<Guid>, bool>> predicate) {
        return await GetRoom(predicate).CountAsync();
    }

    protected override void DefineQuery() {
        base.DefineQuery();

        LoadWithRoomFilter = predicate
            => QueryBuilder(query: Entities,
                spec: h => h.Rooms.AsQueryable().Any(predicate),
                include: h => h
                    .Include(h => h.Rooms.AsQueryable().Where(predicate))
                    .Include(h => h.Tariffs)
            );

        GetRoom = predicate
            => QueryBuilder<Room<Guid>, int>(
                query: Context.Set<Room<Guid>>(),
                spec: predicate,
                options: QueryTune.ChangeTrackingDisable
            );
    }
}