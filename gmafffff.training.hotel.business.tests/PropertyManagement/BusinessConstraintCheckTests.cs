using FluentAssertions;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.PropertyManagement.BusinessConstraintsChecks;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PropertyManagement;

public partial class PropertyManagementTests {
    [TestSubject(typeof(RoomNumberMustUnique))]
    [TestSubject(typeof(RemoveRoomsMustFree))]
    public class BusinessConstraintsChecks(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     бизнес-ограничение <see cref="RoomNumberMustUnique" /> соблюдается при добавлении
        /// </summary>
        /// <returns></returns>
        [Theory]
        [HotelAutodata]
        public async Task RoomNumberMustUniqueByAddIsSuccessful(AddRoomCommand command, Guid guid) {
            // Arrange
            command = command with { Room = command.Room with { Number = guid.ToString() } };
            var check = new RoomNumberMustUnique(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeTrue();
        }

        /// <summary>
        ///     бизнес-ограничение <see cref="RoomNumberMustUnique" /> соблюдается при обновлении
        /// </summary>
        /// <returns></returns>
        [Theory]
        [HotelAutodata]
        public async Task RoomNumberMustUniqueByUpdateIsSuccessful(UpdateRoomCommand command) {
            // Arrange
            var existRoomId = FakeHotel.Hotel.Rooms.First().Id;
            command = command with { Id = existRoomId };

            var check = new RoomNumberMustUnique(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeTrue();
        }

        /// <summary>
        ///     бизнес-ограничение <see cref="RoomNumberMustUnique" /> соблюдается при обновлении несуществующей сущности
        /// </summary>
        /// <returns></returns>
        [Theory]
        [HotelAutodata]
        public async Task RoomNumberMustUniqueByUpdateNotFoundIsSuccessful(UpdateRoomCommand command) {
            // Arrange
            command = command with { Id = -1 };

            var check = new RoomNumberMustUnique(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeTrue();
        }

        /// <summary>
        ///     бизнес-ограничение <see cref="RoomNumberMustUnique" /> нарушается при добавлении
        /// </summary>
        /// <returns></returns>
        [Theory]
        [HotelAutodata]
        public async Task RoomNumberMustUniqueByAddIsFail(AddRoomCommand command) {
            // Arrange
            var existRoomNumber = FakeHotel.Hotel.Rooms.First().RoomDetails.Number;
            command = command with { Room = command.Room with { Number = existRoomNumber } };
            var check = new RoomNumberMustUnique(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeFalse();
        }

        /// <summary>
        ///     бизнес-ограничение <see cref="RoomNumberMustUnique" /> нарушается при обновлении
        /// </summary>
        /// <returns></returns>
        [Theory]
        [HotelAutodata]
        public async Task RoomNumberMustUniqueByUpdateIsFail(UpdateRoomCommand command) {
            // Arrange
            var existRoomId = FakeHotel.Hotel.Rooms.First().Id;
            var existRoomNumber = FakeHotel.Hotel.Rooms.Last().RoomDetails.Number;
            command = command with {
                Id = existRoomId,
                RoomUpdate = command.RoomUpdate with { Number = existRoomNumber }
            };
            var check = new RoomNumberMustUnique(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeFalse();
        }

        /// <summary>
        ///     бизнес-ограничение <see cref="RemoveRoomsMustFree" /> соблюдается
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">если оказалось слишком мало свободных номеров для теста</exception>
        [Fact]
        public async Task RemoveRoomsMustFreeIsSuccess() {
            // Arrange
            var freeRooms = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Compile()).Take(2).ToArray();
            if (freeRooms.Length < 2) throw new NotSupportedException();

            var command = new RemoveRoomsCommand(freeRooms.Select(room => room.Id).ToArray());

            var check = new RemoveRoomsMustFree(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeTrue();
        }


        /// <summary>
        ///     бизнес-ограничение <see cref="RemoveRoomsMustFree" /> нарушается
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">если оказалось слишком мало свободных номеров для теста</exception>
        [Fact]
        public async Task RemoveRoomsMustFreeIsFail() {
            // Arrange
            var busyRoom = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Not().Compile()).First();
            var freeRooms = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Compile()).Take(2).ToArray();
            if (freeRooms.Length < 2) throw new NotSupportedException();

            var command = new RemoveRoomsCommand(freeRooms.Append(busyRoom).Select(room => room.Id).ToArray());

            var check = new RemoveRoomsMustFree(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeFalse();
        }
    }
}