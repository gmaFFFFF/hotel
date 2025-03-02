namespace gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;

public class FakeHotelCustomization : ICustomization {
    public static ushort RoomsCount = 21;

    public void Customize(IFixture fixture) {
        fixture.Customizations.Add(new DateOnlyTimeOnlyGenerator());
        fixture.Customizations.Add(new HotelBlockNameGenerator());
        fixture.Customizations.Add(new PersonFullNameGenerator());
        fixture.Customizations.Add(new RoomDetailsGenerator());
        fixture.Customizations.Add(new RoomTypeGenerator());
        fixture.Customizations.Add(new TariffGenerator());

        fixture.Customize<Room<Guid>>(c => c.Without(room => room.Visit));

        var hotelBlockId = 1;
        fixture.Customize<HotelBlock<int, Guid>>(c =>
            c.With(propertyPicker: p => p.Id, valueFactory: () => hotelBlockId++)
                .With(propertyPicker: p => p.Tariffs,
                    valueFactory: (IFixture f) => [.. f.CreateMany<Tariff>(Enum.GetValues<RoomType>().Length)])
                .With(propertyPicker: p => p.Rooms,
                    valueFactory: (IFixture f) => [..f.CreateMany<Room<Guid>>(RoomsCount)])
        );
    }
}