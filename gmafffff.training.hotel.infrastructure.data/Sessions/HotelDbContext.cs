namespace gmafffff.training.hotel.infrastructure.data.Sessions;

public sealed class HotelDbContext : DbContext {
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) {
        ChangeTracker.StateChanged += RoomConfig.UpdateRowVersion;
    }

    public DbSet<Person<Guid>> Persons { get; set; }
    public DbSet<HotelBlock<int, Guid>> HotelRooms { get; set; }
    public DbSet<AccommodationReport<Guid>> AccommodationReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HotelDbContext).Assembly);
    }
}