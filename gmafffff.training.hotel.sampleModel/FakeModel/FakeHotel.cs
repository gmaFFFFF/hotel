using gmafffff.training.hotel.domain.PersonManagement.Models;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.domain.SettlementManagement.Models;

namespace gmafffff.training.hotel.sampleModel.FakeModel;

public record FakeHotel(
    HotelBlock<int, Guid> Hotel,
    List<AccommodationReport<Guid>> Reports,
    HashSet<Person<Guid>> Persons);