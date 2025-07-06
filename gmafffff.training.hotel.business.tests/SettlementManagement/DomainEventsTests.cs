using FluentAssertions;
using FluentAssertions.Execution;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.DomainEventHandlers;
using gmafffff.training.hotel.domain.Model;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.SettlementManagement;

public partial class SettlementManagementTests {
    [TestSubject(typeof(MoveOutQueueClean))]
    [TestSubject(typeof(MoveOutUpdateHistory))]
    public class DomainEvents(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     После выезда из номера требуется его уборка
        /// </summary>
        [Fact]
        public async Task AfterMoveOutRequiresCleaningRoom() {
            // Arrange
            var room = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Not().Compile()).First();
            var command = new MoveOutCommand(room.Id);

            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<MoveOutCommand>>();

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
        public async Task AfterMoveOutUpdateVisitorHistory() {
            // Arrange
            var room = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Not().Compile()).First();
            var command = new MoveOutCommand(room.Id);
            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<MoveOutCommand>>();
            var oldPersonHistory =
                await PersonRepo.GetAsync(room.Visit.Visitors,
                    entityToDto: person => new { person.Id, person.History });


            // Act
            var res = await runner.Execute(command);

            // Assert
            using var _ = new AssertionScope();
            res.IsSucc.Should().BeTrue();
            var newPersonHistory =
                await PersonRepo.GetAsync(room.Visit.Visitors,
                    entityToDto: person => new { person.Id, person.History });
            foreach (var @new in newPersonHistory) {
                var old = oldPersonHistory.Single(h => h.Id == @new.Id);
                @new.History.Count.Should().Be(old.History.Count + 1);
                @new.History.Duration.Should().BeGreaterThan(old.History.Duration);
            }
        }
    }
}