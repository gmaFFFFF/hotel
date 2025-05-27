using Microsoft.EntityFrameworkCore;

namespace gmafffff.starterKit.tests.Domain.EntityFrameworkCore.Fixtures;

internal class SimpleDbContext(string dbName = nameof(SimpleDbContext)) : DbContext {
    public DbSet<SimpleEntity> SimpleEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseInMemoryDatabase(dbName);
    }
}