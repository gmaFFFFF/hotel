using FluentAssertions;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.PersonManagement.BusinessRules;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.Model;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PersonManagement;

public partial class PersonManagementTests {
    public class BusinessRulesTests(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     Бизнес правило <see cref="RemovePersonShouldNotSuitable" /> соблюдается
        /// </summary>
        [Fact]
        public async Task RemovePersonShouldNotSuitableIsSuccessful() {
            // Arrange
            var busyRooms = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Not().Compile());
            var suitablePersons = busyRooms.SelectMany(room => room.Visit!.Visitors).ToList();
            var leavingPersons = FakeHotel.Persons
                .Select(person => person.Id)
                .Except(suitablePersons)
                .ToArray();
            if (leavingPersons.Length < 2) throw new NotSupportedException();

            var command = new RemovePersonsCommand(leavingPersons);
            var rule = new RemovePersonShouldNotSuitable(HotelRepoFactory);

            // Act, Assert
            (await rule.IsSatisfiedAsync(command))
                .Should().BeTrue();
        }

        /// <summary>
        ///     Бизнес правило <see cref="RemovePersonShouldNotSuitable" /> не соблюдается
        /// </summary>
        [Fact]
        public async Task RemovePersonShouldNotSuitableIsFail() {
            // Arrange
            var busyRooms = FakeHotel.Hotel.Rooms.Where(Room<Guid>.IsFreeRoom.Not().Compile());
            var suitablePersons = busyRooms.SelectMany(room => room.Visit!.Visitors).ToList();
            var leavingPersons = FakeHotel.Persons
                .Select(person => person.Id)
                .Except(suitablePersons)
                .ToArray();
            if (leavingPersons.Length < 2) throw new NotSupportedException();

            leavingPersons[^1] = suitablePersons[0];

            var command = new RemovePersonsCommand(leavingPersons);
            var rule = new RemovePersonShouldNotSuitable(HotelRepoFactory);

            // Act, Assert
            (await rule.IsSatisfiedAsync(command))
                .Should().BeFalse();
        }
    }
}