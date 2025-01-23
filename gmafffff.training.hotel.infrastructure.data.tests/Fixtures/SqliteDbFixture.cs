using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.infrastructure.data.tests.Fixtures;

public class SqliteDbFixture : IDisposable {
    private readonly SqliteConnection _connection;

    private readonly Lazy<StreamWriter> _logStream = new(() =>
        new StreamWriter(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ef_core.log"),
            append: true));

    private Action<string> _logAction = _ => { };
    private DbContextOptions<HotelDbContext> _options;

    public SqliteDbFixture() {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _options = SetOptions();

        using var context = CreateDbContext();
        context.Database.EnsureCreated();
    }


    public StreamWriter DefaultFileLogStream => _logStream.Value;

    public Action<string> LogAction {
        get => _logAction;
        set {
            _logAction = value;
            _options = SetOptions();
        }
    }

    public void Dispose() {
        _connection.Close();
        if (_logStream.IsValueCreated)
            _logStream.Value.Close();
    }

    private DbContextOptions<HotelDbContext> SetOptions() {
        return new DbContextOptionsBuilder<HotelDbContext>()
            .UseSqlite(_connection)
            .LogTo(LogAction, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;
    }

    public HotelDbContext CreateDbContext() {
        return new HotelDbContext(_options);
    }
}