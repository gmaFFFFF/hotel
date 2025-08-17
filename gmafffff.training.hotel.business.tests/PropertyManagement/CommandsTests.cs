using FluentAssertions;
using FluentAssertions.Execution;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.business.PropertyManagement.Handlers;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PropertyManagement;

public partial class PropertyManagementTests {
    [TestSubject(typeof(AddRoomCommandHandler))]
    [TestSubject(typeof(UpdateRoomCommandHandler))]
    [TestSubject(typeof(RemoveRoomCommandHandler))]
    public class Commands(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     Добавляет номер
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task AddRoom(AddRoomCommand command, Guid guid) {
            // Arrange
            command = command with {
                Room = command.Room with {
                    HotelBlockId = 1,
                    Number = guid.ToString()
                }
            };
            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<AddRoomCommand>>();

            // Act
            var result = await runner.Execute(command);

            // Assert
            result.IsSucc.Should().BeTrue();
            var newRoom = (await HotelRepo
                    .GetRoomsAsync(room => room.RoomDetails.Number == command.Room.Number))
                .Single();
            newRoom.Should().BeEquivalentTo(command.Room,
                config: config => config.ExcludingMissingMembers());
        }

        /// <summary>
        ///     Изменяет номер
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task UpdateRoom(UpdateRoomCommand command) {
            // Arrange
            var old = FakeHotel.Hotel.Rooms.First();
            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<UpdateRoomCommand>>();
            command = command with { Id = old.Id };

            // Act
            var result = await runner.Execute(command);
            var updatedRoom = (await HotelRepo.GetRoomsAsync(room => room.Id == command.Id)
                ).Single();

            // Assert
            result.IsSucc.Should().BeTrue();
            updatedRoom.Should().BeEquivalentTo(command.RoomUpdate,
                config: config => config.ExcludingMissingMembers());
        }

        /// <summary>
        ///     Удаляет номер
        /// </summary>
        /// <exception cref="NotSupportedException">Если оказалось слишком мало свободных номеров для теста</exception>
        [Fact]
        public async Task RemoveRoom() {
            // Arrange
            var removeRooms = FakeHotel.Hotel.Rooms.Where(Room<Guid>.WhereFreeRoom.Compile()).Take(2).ToArray();
            var removeRoomsIds = removeRooms.Select(room => room.Id).ToArray();
            if (removeRooms.Length < 2) throw new NotSupportedException();

            var command = new RemoveRoomsCommand(removeRoomsIds);
            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<RemoveRoomsCommand>>();

            // Act
            var result = await runner.Execute(command);

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            result.SuccSpan()[0]
                .OfType<DeletedBusinessEvent<int>>()
                .Select(e => new { Id = e.EntityId })
                .Should().BeEquivalentTo(removeRooms.Select(r => new { r.Id }));

            (await HotelRepo.GetRoomsAsync(room => removeRoomsIds.Contains(room.Id)))
                .Should().BeEmpty();
        }
    }
}