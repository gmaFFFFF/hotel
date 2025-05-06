using JetBrains.Annotations;

namespace gmafffff.training.hotel.domain.tests.Tests;

public partial class SettlementTests {
    [TestSubject(typeof(HotelBlock<,>))]
    public sealed class MoveOut {
        /// <summary>
        ///     После выселения номер становится доступным для заселения
        /// </summary>
        [Theory]
        [HotelAutodata]
        public void AfterCheckOutRoomAvailableForCheckIn(Person<Guid> person, HotelBlock<int, Guid> hotel) {
            // Arrange
            var room = hotel.FindSuitable(RoomType.None, capacity: 1).First();

            // Act
            hotel.SettledIn([person.Id], room);

            // Assert
            var _ = new AssertionScope();

            room.IsFree.Should().BeTrue();

            hotel.FindSuitable(RoomType.None, capacity: 1).Should()
                .Contain(room);
        }

        /// <summary>
        ///     Отчет содержит корректную информацию о проживании
        /// </summary>
        [Theory]
        [HotelAutodata]
        public void ReportContainsCorrectInformationAboutAccommodation(IEnumerable<Person<Guid>> persons,
            HotelBlock<int, Guid> hotel, byte arrival, byte duration) {
            var arrivalDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-arrival);
            var departureDate = arrivalDate.AddDays(duration);

            // Arrange
            var room = hotel.FindSuitable(RoomType.None, capacity: 1).MaxBy(r => r.RoomDetails.Type);

            // Act
            hotel.SettledIn(persons.Select(x => x.Id), room, arrivalDate);
            var report = hotel.MoveOut(room, departureDate);

            // Assert
            var _ = new AssertionScope();
            var tariff = hotel.Tariffs.Single(t => t.TariffDetails.Type == room.RoomDetails.Type).TariffDetails;
            var price = tariff.Value;

            report.ArrivalDate.Should().Be(arrivalDate);
            report.DepartureDate.Should().Be(departureDate);
            report.Duration.Should().Be(duration);
            report.Price.Should().Be(price);
            report.RoomDetails.Should().Be(room.RoomDetails);
            report.TariffDetails.Should().Be(tariff);
            report.Visitors.Should().BeEquivalentTo(persons.Select(x => x.Id));
        }
    }
}