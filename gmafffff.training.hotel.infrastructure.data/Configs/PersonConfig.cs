namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class PersonConfig : IEntityTypeConfiguration<Person<Guid>> {
    public const string EntityName = "Person";
    private const string PrimaryKey = $"{EntityName}Id";

    public void Configure(EntityTypeBuilder<Person<Guid>> builder) {
        // Наименования
        builder.Property(x => x.Id)
            .HasColumnName(PrimaryKey);

        // Генераторы значений
        builder.Property(x => x.Id)
            .HasValueGenerator<SequentialGuidValueGenerator>();

        // Комплексные свойства
        builder.ComplexProperty(x => x.FullName);
    }
}