using FluentAssertions;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.SettlementManagement.BusinessConstraintsChecks;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.SettlementManagement;

public partial class SettlementManagementTests {
    [TestSubject(typeof(NumberVisitorsNotExceedCapacityRoom))]
    public class BusinessConstraintsChecks(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     бизнес-ограничение <see cref="NumberVisitorsNotExceedCapacityRoom" /> соблюдается
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task NumberVisitorsNotExceedCapacityRoomIsSuccessful(SettleInCommand command) {
            // Arrange
            var room = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.WhereFreeRoom.Not().Compile())
                .FirstOrDefault(r => r.RoomDetails.Capacity > r.Visit!.Visitors.Count);
            if (room is null)
                throw new Exception("Не сложились обстоятельства для теста. Повторите запуск");
            var visitors = FakeHotel.Persons
                .Where(p => !room.Visit!.Visitors.Contains(p.Id))
                .Take(room.RoomDetails.Capacity - room.Visit!.Visitors.Count).Select(v => v.Id);

            command = command with { RoomId = room.Id, Visitors = visitors };

            // Act
            var check = new NumberVisitorsNotExceedCapacityRoom(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeTrue();
        }

        /// <summary>
        ///     бизнес-ограничение <see cref="NumberVisitorsNotExceedCapacityRoom" /> не соблюдается
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task NumberVisitorsNotExceedCapacityRoomIsFail(SettleInCommand command) {
            // Arrange
            var room = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.WhereFreeRoom.Not().Compile())
                .FirstOrDefault(r => r.RoomDetails.Capacity > r.Visit!.Visitors.Count);
            if (room is null)
                throw new Exception("Не сложились обстоятельства для теста. Повторите запуск");
            var visitors = FakeHotel.Persons
                .Where(p => !room.Visit!.Visitors.Contains(p.Id))
                .Take(room.RoomDetails.Capacity - room.Visit!.Visitors.Count + 1).Select(v => v.Id);

            command = command with { RoomId = room.Id, Visitors = visitors };

            // Act
            var check = new NumberVisitorsNotExceedCapacityRoom(HotelRepoFactory);

            // Act, Assert
            (await check.IsSatisfiedAsync(command))
                .Should().BeFalse();
        }
    }
}