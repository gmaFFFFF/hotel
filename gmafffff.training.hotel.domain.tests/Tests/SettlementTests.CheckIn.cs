using JetBrains.Annotations;

namespace gmafffff.training.hotel.domain.tests.Tests;

public partial class SettlementTests {
    [TestSubject(typeof(HotelBlock<,>))]
    public sealed class CheckIn {
        /// <summary>
        ///     Можно заселить человека в номер
        /// </summary>
        [Theory]
        [HotelAutodata]
        public void CanOccupyPersonInRoom(Person<Guid> person, HotelBlock<int, Guid> hotel, byte duration) {
            var departureDate = DateOnly.FromDateTime(DateTime.Today).AddDays(duration);

            // Arrange
            var room = hotel.FindSuitable(RoomType.None, capacity: 1).MinBy(r => r.RoomDetails.Type);

            // Act
            hotel.SettledIn([person.Id], room, departureDatePlanned: departureDate);

            // Assert
            var _ = new AssertionScope();

            room.IsFree.Should().BeFalse();

            hotel.FindSuitable(RoomType.None, capacity: 1).Should()
                .NotContain(room);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.ArrivalDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.TariffDetails.Should()
                .Be(hotel.Tariffs.Single(t => t.TariffDetails.Type == room.RoomDetails.Type).TariffDetails);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.DepartureDatePlanned.Should().Be(departureDate);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.Visitors.Should()
                .ContainSingle()
                .Which.Should().Be(person.Id);
        }

        /// <summary>
        ///     Можно заселить несколько человек в номер
        /// </summary>
        [Theory]
        [HotelAutodata]
        public void CanOccupyManyPersonsInRoom(IEnumerable<Person<Guid>> persons, HotelBlock<int, Guid> hotel,
            byte duration) {
            var departureDate = DateOnly.FromDateTime(DateTime.Today).AddDays(duration);
            var yesterday = DateOnly.FromDateTime(DateTime.Today).AddDays(-1);

            // Arrange
            var room = hotel.FindSuitable(RoomType.None, capacity: 1).MaxBy(r => r.RoomDetails.Type);

            // Act
            hotel.SettledIn(persons.Select(x => x.Id), room, yesterday, departureDate);

            // Assert
            var _ = new AssertionScope();

            room.IsFree.Should().BeFalse();

            hotel.FindSuitable(RoomType.None, capacity: 1).Should()
                .NotContain(room);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.ArrivalDate.Should().Be(yesterday);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.TariffDetails.Should()
                .Be(hotel.Tariffs.Single(t => t.TariffDetails.Type == room.RoomDetails.Type).TariffDetails);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.DepartureDatePlanned.Should().Be(departureDate);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.Visitors.Should()
                .BeEquivalentTo(persons.Select(x => x.Id));
        }

        /// <summary>
        ///     Можно подселить людей в номер
        /// </summary>
        [Theory]
        [HotelAutodata]
        public void CanHookUpManyPersonsInRoom(IEnumerable<Person<Guid>> firstPersons,
            IEnumerable<Person<Guid>> latestPersons,
            HotelBlock<int, Guid> hotel, byte duration) {
            var departureDate = DateOnly.FromDateTime(DateTime.Today).AddDays(duration);
            var yesterday = DateOnly.FromDateTime(DateTime.Today).AddDays(-1);
            var allPersons = firstPersons.Concat(latestPersons);

            // Arrange
            var room = hotel.FindSuitable(RoomType.None, capacity: 1).MaxBy(r => r.RoomDetails.Type);

            // Act
            hotel.SettledIn(firstPersons.Select(x => x.Id), room, yesterday, departureDate);
            hotel.SettledIn(latestPersons.Select(x => x.Id), room, yesterday, departureDate);

            // Assert
            var _ = new AssertionScope();

            room.IsFree.Should().BeFalse();

            hotel.FindSuitable(RoomType.None, capacity: 1).Should()
                .NotContain(room);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.ArrivalDate.Should().Be(yesterday);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.TariffDetails.Should()
                .Be(hotel.Tariffs.Single(t => t.TariffDetails.Type == room.RoomDetails.Type).TariffDetails);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.DepartureDatePlanned.Should().Be(departureDate);

            hotel.Rooms.Should()
                .ContainSingle(r => r.Visit != null)
                .Which.Visit.Visitors.Should()
                .BeEquivalentTo(allPersons.Select(x => x.Id));
        }
    }
}