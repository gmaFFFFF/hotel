namespace gmafffff.training.hotel.sampleModel.FakeModel;

public record FakeHotel(
    HotelBlock<int, Guid> Hotel,
    List<AccommodationReport<Guid>> Reports,
    HashSet<Person<Guid>> Persons);