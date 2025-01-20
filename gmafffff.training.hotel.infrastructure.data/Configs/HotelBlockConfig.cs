namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class HotelBlockConfig : IEntityTypeConfiguration<HotelBlock<int, Guid>> {
    public const string PrimaryKey = "HotelBlockId";

    public void Configure(EntityTypeBuilder<HotelBlock<int, Guid>> builder) {
        // Наименования
        builder.Property(x => x.Id)
            .HasColumnName(PrimaryKey);
    }
}