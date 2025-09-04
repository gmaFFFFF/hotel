using FluentAssertions;
using FluentAssertions.Execution;
using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.business.SettlementManagement.Events;
using gmafffff.training.hotel.business.SettlementManagement.Handlers;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.SettlementManagement;

public partial class SettlementManagementTests {
    [TestSubject(typeof(SettleInCommandHandler))]
    [TestSubject(typeof(MoveOutCommandHandler))]
    public class CommandsTests(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     Возможно заселение в пустой номер
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task PossibleSettleInEmptyRoom(SettleInCommand command) {
            // Arrange
            var room = FakeHotel.Hotel.Rooms.Where(Room<Guid>.WhereFreeRoom.Compile()).First();
            var visitors = FakeHotel.Persons.Take(room.RoomDetails.Capacity).Select(v => v.Id);

            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<SettleInCommand>>();
            command = command with { RoomId = room.Id, Visitors = visitors, DepartureDatePlanned = null };

            // Act
            var result = await runner.ExecuteAsync(command);

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            result.IfSucc(e => e.OfType<SettledInEvent>().Single().RoomId.Should().Be(command.RoomId));
            var saveRoom = (await HotelRepo.LoadWithRoomFilterAsync(r => r.Id == room.Id))
                .Single()
                .Rooms.Single();
            saveRoom.Visit.Should().NotBeNull();
            saveRoom.Visit!.Visitors.Should().BeEquivalentTo(visitors);
            saveRoom.Visit.ArrivalDate = DateOnly.FromDateTime(DateTime.Today);
            saveRoom.Visit.DepartureDatePlanned = command.DepartureDatePlanned;
        }

        /// <summary>
        ///     Возможно подселение
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task PossibleSettleInRoom(SettleInCommand command) {
            // Arrange
            var room = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.WhereFreeRoom.Not().Compile())
                .FirstOrDefault(r => r.RoomDetails.Capacity > r.Visit!.Visitors.Count);
            if (room is null)
                throw new Exception("Не сложились обстоятельства для теста. Повторите запуск");
            var visitors = FakeHotel.Persons
                .Where(p => !room.Visit!.Visitors.Contains(p.Id))
                .Take(room.RoomDetails.Capacity - room.Visit!.Visitors.Count).Select(v => v.Id);

            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<SettleInCommand>>();
            command = command with { RoomId = room.Id, Visitors = visitors, DepartureDatePlanned = null };

            // Act
            var result = await runner.ExecuteAsync(command);

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            result.IfSucc(e => e.OfType<SettledInEvent>().Single().RoomId.Should().Be(command.RoomId));
            var saveRoom = (await HotelRepo.LoadWithRoomFilterAsync(r => r.Id == room.Id))
                .Single()
                .Rooms.Single();
            saveRoom.Visit.Should().NotBeNull();
            saveRoom.Visit!.Visitors.Should().BeEquivalentTo(visitors.Concat(room.Visit!.Visitors));
            saveRoom.Visit.ArrivalDate = DateOnly.FromDateTime(DateTime.Today);
            saveRoom.Visit.DepartureDatePlanned = command.DepartureDatePlanned;
        }

        /// <summary>
        ///     Сведения о посетители должны быть учтены в БД
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task VisitorMustBeInDb(SettleInCommand command) {
            // Arrange
            var room = FakeHotel.Hotel.Rooms.Where(Room<Guid>.WhereFreeRoom.Compile()).First();
            var visitors = FakeHotel.Persons.Take(room.RoomDetails.Capacity).Select(v => v.Id).ToArray();
            visitors[0] = Guid.NewGuid();

            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<SettleInCommand>>();
            command = command with { RoomId = room.Id, Visitors = visitors, DepartureDatePlanned = null };

            // Act
            var result = await runner.ExecuteAsync(command);

            // Assert
            result.IsSucc.Should().BeFalse();
            result.IfFail(err => err.Code.Should().Be((int)AppErrorCode.DbNotFound));
        }

        /// <summary>
        ///     Возможен выезд из номера
        /// </summary>
        [Fact]
        public async Task PossibleMoveOut() {
            // Arrange
            var rooms = FakeHotel.Hotel.Rooms.Where(Room<Guid>.WhereFreeRoom.Not().Compile())
                .Take(2)
                .ToArray();
            var command1 = new MoveOutCommand(rooms[0].Id, DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));
            var command2 = new MoveOutCommand(rooms[1].Id);

            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<MoveOutCommand>>();

            // Act
            var res1 = await runner.ExecuteAsync(command1);
            var res2 = await runner.ExecuteAsync(command2);

            // Assert
            using var _ = new AssertionScope();
            res1.IsSucc.Should().BeTrue();
            var report1 = res1.Map(be => be.OfType<MovedOutEvent>().ToArray()).ThrowIfFail()[0].Report;
            report1.RoomDetails.Should().Be(rooms[0].RoomDetails);
            report1.Visitors.Should().BeEquivalentTo(rooms[0].Visit.Visitors);
            report1.ArrivalDate.Should().Be(rooms[0].Visit.ArrivalDate);
            report1.TariffDetails.Should().Be(rooms[0].Visit.TariffDetails);
            report1.DepartureDate.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));

            res2.IsSucc.Should().BeTrue();
            res2.Map(be => be.OfType<MovedOutEvent>().ToArray()).ThrowIfFail()[0].Report.DepartureDate.Should()
                .Be(DateOnly.FromDateTime(DateTime.Today));
        }
    }
}