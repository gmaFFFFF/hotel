using gmafffff.training.hotel.infrastructure.data.pay.Sessions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.SampleModel.FakeDb;

public class SqliteInvoiceDbFixture : IDisposable {
    private readonly SqliteConnection _connection;

    private readonly Lazy<StreamWriter> _logStream = new(() =>
        new StreamWriter(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"{typeof(SqliteHotelDbFixture).Assembly.GetName().Name}.log"),
            append: true));

    private Action<string> _logAction = _ => { };
    private DbContextOptions<InvoiceDbContext> _options;

    public SqliteInvoiceDbFixture() {
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
            _logAction = value ?? (_ => { });
            _options = SetOptions();
        }
    }

    public void Dispose() {
        _connection.Close();
        if (_logStream.IsValueCreated)
            _logStream.Value.Close();
    }

    private DbContextOptions<InvoiceDbContext> SetOptions() {
        return new DbContextOptionsBuilder<InvoiceDbContext>()
            .UseSqlite(_connection)
            .LogTo(LogAction, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .ConfigureWarnings(builder => builder.Log(RelationalEventId.AmbientTransactionWarning))
            .Options;
    }

    public InvoiceDbContext CreateDbContext() {
        return new InvoiceDbContext(_options);
    }
}