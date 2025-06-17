using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Xunit.Abstractions;

using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.DomainEventHandlers;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.tests.SettlementManagement;

public partial class SettlementManagementTests
{
    [TestSubject(typeof(MoveOutQueueClean))]
    [TestSubject(typeof(MoveOutUpdateHistory))]
    public class DomainEvents(ITestOutputHelper output) : TestContext(output)
    {
        /// <summary>
        ///     После выезда из номера требуется его уборка
        /// </summary>
        [Fact]
        public async Task AfterMoveOutRequiresCleaningRoom()
        {
            // Arrange
            var room = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Not().Compile()).First();
            var command = new MoveOutCommand(room.Id);

            var runner = new BusinessActionRunner<MoveOutCommand>(Scope.ServiceProvider);

            // Act
            var res = await runner.Execute(command);

            // Assert
            using var _ = new AssertionScope();
            res.IsSucc.Should().BeTrue();

            var dirtyRoom = (await HotelRepo.GetAsync())
                .Single()
                .Rooms.Single(Room<Guid>.IsCleanRoom.Not().Compile());
            dirtyRoom.Id.Should().Be(room.Id);
        }

        /// <summary>
        ///     После выезда из номера у посетителя обновляется статистика посещения гостиницы
        /// </summary>
        [Fact]
        public async Task AfterMoveOutUpdateVisitorHistory()
        {
            // Arrange
            var room = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Not().Compile()).First();
            var command = new MoveOutCommand(room.Id);
            var runner = new BusinessActionRunner<MoveOutCommand>(Scope.ServiceProvider);
            var oldPersonHistory = await PersonRepo.GetAsync(room.Visit.Visitors, person => new { person.Id, person.History });


            // Act
            var res = await runner.Execute(command);

            // Assert
            using var _ = new AssertionScope();
            res.IsSucc.Should().BeTrue();
            var newPersonHistory = await PersonRepo.GetAsync(room.Visit.Visitors, person => new { person.Id, person.History });
            foreach (var @new in newPersonHistory) {
                var old = oldPersonHistory.Single(h => h.Id == @new.Id);
                @new.History.Count.Should().Be(old.History.Count + 1);
                @new.History.Duration.Should().BeGreaterThan(old.History.Duration);
            }
        }
    }
}