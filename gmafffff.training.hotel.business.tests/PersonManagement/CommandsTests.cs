using FluentAssertions;
using FluentAssertions.Execution;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PersonManagement;

public partial class PersonManagementTests {
    public class CommandsTests(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     Добавляет персону
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task AddPerson(AddPersonCommand command) {
            // Arrange
            var runner = new BusinessActionRunner<AddPersonCommand, CreatedBusinessEvent<Guid>>(Scope.ServiceProvider);

            // Act
            var result = await runner.Execute(command);
            var newPerson = (await PersonRepo
                    .GetPersonsAsync(person => person.FullName.FirstName == command.Person.FirstName &&
                                               person.FullName.SurName == command.Person.SurName &&
                                               person.FullName.Patronymic == command.Person.Patronymic)
                ).Single();

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            newPerson.Should().BeEquivalentTo(command.Person,
                config: config => config.ExcludingMissingMembers());
        }

        /// <summary>
        ///     Изменяет персону
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task UpdatePerson(UpdatePersonCommand command) {
            // Arrange
            var old = FakeHotel.Persons.First();
            var runner =
                new BusinessActionRunner<UpdatePersonCommand, UpdatedBusinessEvent<Guid>>(Scope.ServiceProvider);
            command = command with { Id = old.Id };

            // Act
            var result = await runner.Execute(command);
            var updatedPerson = (await PersonRepo.GetPersonsAsync(person => person.Id == command.Id)
                ).Single();

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            updatedPerson.Should().BeEquivalentTo(command.PersonUpdate,
                config: config => config.ExcludingMissingMembers());
        }

        /// <summary>
        ///     Удаляет персону
        /// </summary>
        [Fact]
        public async Task RemovePerson() {
            // Arrange
            var busyRooms = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Not().Compile());
            var suitablePersons = busyRooms.SelectMany(room => room.Visit!.Visitors).ToList();
            var leavingPersons = FakeHotel.Persons
                .Select(person => person.Id)
                .Except(suitablePersons)
                .ToArray();
            if (leavingPersons.Length < 2)
                throw new NotSupportedException("Если не сложилась тестовая ситуация нужно просто перезапустить тест");

            var command = new RemovePersonsCommand(leavingPersons);
            var runner =
                new BusinessActionRunner<RemovePersonsCommand, RemovedBusinessEvent<Guid>>(Scope.ServiceProvider);

            // Act
            var result = await runner.Execute(command);

            // Assert
            var removePersonIds = result.IfFail(_ => [])
                .Select(e => e.EntityId);

            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            (await PersonRepo.GetPersonsAsync(person => removePersonIds.Contains(person.Id)))
                .Should().BeEmpty();
        }
    }
}