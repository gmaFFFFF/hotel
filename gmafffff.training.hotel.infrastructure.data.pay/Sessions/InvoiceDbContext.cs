using EntityFramework.Exceptions.Sqlite;
using gmafffff.starterKit.EntityFrameworkCore.Conventions;
using gmafffff.training.hotel.domain.pay.Model;
using Microsoft.EntityFrameworkCore;

namespace gmafffff.training.hotel.infrastructure.data.pay.Sessions;

public class InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : DbContext(options) {
    public DbSet<Invoice> Invoices { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
        configurationBuilder.Properties<Enum>()
            .HaveConversion<string>();

        if (Database.IsSqlite())
            configurationBuilder.Properties<decimal>()
                .HaveConversion<double>();

        configurationBuilder.Conventions.Add(_ => new PrimaryKeyNameConventionIsEntityNameId());
        configurationBuilder.Conventions.Add(_ => new ForeignKeyNameConventionIsParentEntityNameParentColumnName());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseExceptionProcessor();
    }
}