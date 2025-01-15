namespace gmafffff.training.hotel.domain.tests.Tests;

public partial class SettlementTests {
    public sealed class RoomSearch {
        private readonly IFixture _fixture = new Fixture().Customize(new FakeHotelCustomization());

        /// <summary>
        ///     Находит свободные номера
        /// </summary>
        [Theory]
        [HotelInlineAutodata(RoomType.Standard, 2)]
        [HotelInlineAutodata(RoomType.SemiLux, 2)]
        [HotelInlineAutodata(RoomType.Lux, 2)]
        public void FindAvailableRooms(RoomType type, byte capacity, HotelBlock<int, Guid> hotel) {
            // Act
            var rooms = hotel.FindSuitable(type, capacity);

            // Assert
            rooms.Should()
                .BeEquivalentTo(
                    hotel.Rooms.Where(r => r.RoomDetails.Type >= type && r.RoomDetails.Capacity >= capacity));
        }

        /// <summary>
        ///     При поиске учитывает вместительность номера
        /// </summary>
        [Theory]
        [HotelAutodata]
        public void WhenSearchingConsiderCapacity(Room<Guid> room, HotelBlock<int, Guid> hotel) {
            var capacity = byte.MaxValue;

            // Arrange
            room.RoomDetails = room.RoomDetails with { Capacity = capacity };
            hotel.Rooms.Add(room);

            // Act
            var rooms = hotel.FindSuitable(RoomType.None, capacity);

            // Assert
            rooms.Should().ContainSingle()
                .Which.Should().Be(room);
        }

        /// <summary>
        ///     При поиске учитывает категорию номера
        /// </summary>
        [Theory]
        [HotelAutodata]
        public void WhenSearchingConsiderType(Room<Guid> room, HotelBlock<int, Guid> hotel) {
            var type = RoomType.SemiLux;

            // Arrange
            room.RoomDetails = room.RoomDetails with { Type = type };
            hotel.Rooms = hotel.Rooms.Where(room => room.RoomDetails.Type < type).ToList();
            hotel.Rooms.Add(room);

            // Act
            var rooms = hotel.FindSuitable(type, capacity: 0);

            // Assert
            rooms.Should().ContainSingle()
                .Which.Should().Be(room);
        }

        /// <summary>
        ///     При поиске учитывает текущих жильцов
        /// </summary>
        [Theory]
        [HotelAutodata]
        public void WhenSearchingConsiderBusy(Room<Guid> room, HotelBlock<int, Guid> hotel) {
            // Arrange
            foreach (var r in hotel.Rooms)
                r.Visit = _fixture.Create<RoomVisit<Guid>>();

            hotel.Rooms.Add(room);

            // Act
            var rooms = hotel.FindSuitable(RoomType.None, capacity: 0);

            // Assert
            rooms.Should().ContainSingle()
                .Which.Should().Be(room);
        }
    }
}