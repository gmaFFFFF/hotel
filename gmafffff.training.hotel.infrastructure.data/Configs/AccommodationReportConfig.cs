namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class AccommodationReportConfig : IEntityTypeConfiguration<AccommodationReport<Guid>> {
    public void Configure(EntityTypeBuilder<AccommodationReport<Guid>> builder) {
        // Комплексные свойства
        builder.ComplexProperty(x => x.RoomDetails);
        builder.ComplexProperty(x => x.TariffDetails);
    }
}