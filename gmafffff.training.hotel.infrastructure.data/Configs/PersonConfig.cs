using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class PersonConfig : IEntityTypeConfiguration<Person<Guid>> {
    public void Configure(EntityTypeBuilder<Person<Guid>> builder) {
        // Генераторы значений
        builder.Property(x => x.Id)
            .HasValueGenerator<SequentialGuidValueGenerator>();

        // Комплексные свойства
        builder.ComplexProperty(x => x.FullName);
        builder.ComplexProperty(x => x.History);
    }
}