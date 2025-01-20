namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class TariffConfig : IEntityTypeConfiguration<Tariff> {
    private const string PrimaryKey = $"{nameof(Tariff)}Id";
    public const string HotelBlockForeignKey = HotelBlockConfig.PrimaryKey;

    public void Configure(EntityTypeBuilder<Tariff> builder) {
        // Теневые свойства
        builder.Property<int>(PrimaryKey);

        // Комплексные свойства
        builder.ComplexProperty(x => x.TariffDetails);

        // Ключи
        builder.HasKey(PrimaryKey);

        // Связи
        builder.Property<int>(HotelBlockForeignKey)
            .IsRequired()
            .HasColumnName(HotelBlockForeignKey);
    }
}