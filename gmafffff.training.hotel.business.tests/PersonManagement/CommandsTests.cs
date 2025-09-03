using FluentAssertions;
using FluentAssertions.Execution;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.business.PersonManagement.Handlers;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.sampleModel.FakeModel.AutoFixture;
using JetBrains.Annotations;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PersonManagement;

public partial class PersonManagementTests {
    [TestSubject(typeof(AddPersonCommandHandler))]
    [TestSubject(typeof(UpdatePersonCommandHandler))]
    [TestSubject(typeof(RemovePersonsCommandHandler))]
    public class Commands(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     Добавляет персону
        /// </summary>
        [Theory]
        [HotelAutodata]
        public async Task AddPerson(AddPersonCommand command) {
            // Arrange
            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<AddPersonCommand>>();

            // Act
            var result = await runner.ExecuteAsync(command);
            var newPerson = (await PersonRepo
                    .GetAsync(spec: person => person.FullName.FirstName == command.New.FirstName &&
                                              person.FullName.SurName == command.New.SurName &&
                                              person.FullName.Patronymic == command.New.Patronymic)
                )
                .Single()
                .Adapt<PersonDto>(PersonDto.MapperConfig);

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            newPerson.Should().BeEquivalentTo(command.New,
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
            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<UpdatePersonCommand>>();
            command = command with { Id = old.Id };

            // Act
            var result = await runner.ExecuteAsync(command);
            var updatedPerson = (await PersonRepo.GetAsync(person => person.Id == command.Id)
                )
                .Single()
                .Adapt<PersonDto>(PersonDto.MapperConfig);

            // Assert
            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            updatedPerson.Should().BeEquivalentTo(command.Changed,
                config: config => config.ExcludingMissingMembers());
        }

        /// <summary>
        ///     Удаляет персону
        /// </summary>
        [Fact]
        public async Task RemovePerson() {
            // Arrange
            var busyRooms = FakeHotel.Hotel.Rooms.Where(Room<Guid>.WhereFreeRoom.Not().Compile());
            var suitablePersons = busyRooms.SelectMany(room => room.Visit!.Visitors).ToList();
            var leavingPersons = FakeHotel.Persons
                .Select(person => person.Id)
                .Except(suitablePersons)
                .ToArray();

            if (leavingPersons.Length < 2)
                throw new NotSupportedException("Если не сложилась тестовая ситуация нужно просто перезапустить тест");

            var command = new RemovePersonsCommand(leavingPersons);
            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<RemovePersonsCommand>>();

            // Act
            var result = await runner.ExecuteAsync(command);

            // Assert
            var removePersonIds = result.IfFail(_ => [])
                .OfType<DeletedBusinessEvent<Guid>>()
                .Select(e => e.EntityId);

            using var _ = new AssertionScope();
            result.IsSucc.Should().BeTrue();
            (await PersonRepo.GetAsync(person => removePersonIds.Contains(person.Id)))
                .Should().BeEmpty();
        }
    }
}