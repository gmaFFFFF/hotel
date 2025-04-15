namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class AccommodationReportConfig : IEntityTypeConfiguration<AccommodationReport<Guid>> {
    public const string EntityName = "AccommodationReport";
    public const string PrimaryKey = $"{EntityName}Id";

    public void Configure(EntityTypeBuilder<AccommodationReport<Guid>> builder) {
        // Наименования
        builder.Property(x => x.Id)
            .HasColumnName(PrimaryKey);

        // Комплексные свойства
        builder.ComplexProperty(x => x.RoomDetails);
        builder.ComplexProperty(x => x.TariffDetails);
    }
}