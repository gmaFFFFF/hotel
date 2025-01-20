namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class RoomConfig : IEntityTypeConfiguration<Room<Guid>> {
    public const string EntityName = "Room";
    public const string PrimaryKey = $"{EntityName}Id";
    public const string HotelBlockForeignKey = HotelBlockConfig.PrimaryKey;
    public const string RoomVisitTable = "RoomVisit";
    public const string RowVersionCol = "rowVersion";

    public void Configure(EntityTypeBuilder<Room<Guid>> builder) {
        // Наименования
        builder.ToTable($"{EntityName}s");
        builder.Property(x => x.Id)
            .HasColumnName(PrimaryKey);

        // Теневые свойства

        // Комплексные свойства
        builder.ComplexProperty(x => x.RoomDetails)
            .Property(x => x.Type)
            .HasConversion<string>();

        // Принадлежащие сущности
        builder.OwnsOne(navigationExpression: x => x.Visit, buildAction: v => {
            // Наименования
            v.ToTable(RoomVisitTable);

            // Теневые свойства
            v.Property<int>(PrimaryKey)
                .HasColumnName(PrimaryKey);

            // Принадлежащие сущности
            v.OwnsOne(x => x.TariffDetails);

            // Ключи

            // Маркер параллелизма
            v.Property<int>(RowVersionCol)
                .HasDefaultValue(0)
                .IsConcurrencyToken();
        });

        // Ключи

        // Индексы и ограничения
        // Нельзя добавить уникальный индекс (№ номера) в комплексном свойстве, а ещё и связать его с FK
        // TODO: Добавить уникальный индекс вручную в миграциях

        // Маркер параллелизма

        // Связи
        builder.Property<int>(HotelBlockForeignKey)
            .HasColumnName(HotelBlockForeignKey)
            .IsRequired();
    }

    /// <summary>
    ///     Обработчик события, предназначенный для инкремента rowVersion при изменении статуса сущности на Modified
    /// </summary>
    /// <remarks> Используется для ручного управления маркером параллелизмом в SQLite, т.к. бд «из коробки» этого не умеет</remarks>
    public static void UpdateRowVersion(object? sender, EntityStateChangedEventArgs e) {
        if (e.Entry.Entity is RoomVisit<Guid> && e.NewState is EntityState.Modified)
            e.Entry.Property(RowVersionCol).CurrentValue = (int)e.Entry.Property(RowVersionCol).OriginalValue + 1;
    }
}