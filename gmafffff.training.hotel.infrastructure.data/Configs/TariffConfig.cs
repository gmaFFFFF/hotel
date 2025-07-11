using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.domain.SettlementManagement.Models;

namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class TariffConfig : IEntityTypeConfiguration<Tariff> {
    public void Configure(EntityTypeBuilder<Tariff> builder) {
        // Теневые свойства
        builder.Property<int>("Id");

        // Комплексные свойства
        builder.ComplexProperty(x => x.TariffDetails);

        // Связи
        builder
            .HasOne<HotelBlock<int, Guid>>()
            .WithMany(h => h.Tariffs)
            .IsRequired();
    }
}