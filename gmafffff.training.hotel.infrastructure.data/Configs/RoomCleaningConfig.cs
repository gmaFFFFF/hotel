using gmafffff.training.hotel.domain.SettlementManagement.Models;

namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class RoomCleaningConfig : IEntityTypeConfiguration<RoomCleaning> {
    public void Configure(EntityTypeBuilder<RoomCleaning> builder) {
        builder.Property(cleaning => cleaning.IsClean)
            .HasComputedColumnSql("CleaningCompleted IS NOT NULL");
    }
}