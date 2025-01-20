using Microsoft.EntityFrameworkCore.Design;

namespace gmafffff.training.hotel.infrastructure.data.Sessions;

/// <summary>
///     Вспомогательный класс для создания миграций
/// </summary>
public class SqliteDesignerAuxiliary : IDesignTimeDbContextFactory<HotelDbContext> {
    public HotelDbContext CreateDbContext(string[] args) {
        var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
        optionsBuilder.UseSqlite("Data Source=designer.db");

        return new HotelDbContext(optionsBuilder.Options);
    }
}