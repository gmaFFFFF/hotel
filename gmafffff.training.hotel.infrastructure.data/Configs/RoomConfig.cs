namespace gmafffff.training.hotel.infrastructure.data.Configs;

public class RoomConfig : IEntityTypeConfiguration<Room<Guid>> {
    public const string EntityName = "Room";
    public const string PrimaryKey = $"{EntityName}Id";
    public const string RoomVisitTable = "RoomVisit";
    public const string RowVersionCol = "rowVersion";

    public void Configure(EntityTypeBuilder<Room<Guid>> builder) {
        // Наименования
        builder.ToTable($"{EntityName}s");

        // Теневые свойства

        // Комплексные свойства
        builder.ComplexProperty(x => x.RoomDetails);

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
        // Добавлен уникальный индекс в миграции AddRoomUniqueKey

        // Маркер параллелизма

        // Связи
        builder
            .HasOne<HotelBlock<int, Guid>>()
            .WithMany(h => h.Rooms)
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