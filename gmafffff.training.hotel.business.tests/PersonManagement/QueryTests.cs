using FluentAssertions;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.PersonManagement.Queries;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PersonManagement;

public partial class PersonManagementTests {
    public class Query(ITestOutputHelper output) : TestContext(output) {
        [Fact]
        public async Task CanQueryData() {
            // Arrange
            var exceptedIds = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.IsFreeRoom.Not().Compile())
                .SelectMany(r => r.Visit!.Visitors)
                .ToArray();
            var excepted = FakeHotel.Persons
                .Where(p => exceptedIds.Contains(p.Id))
                .Select(person => person.Adapt<PersonDto>())
                .ToArray();
            var query = new GetPersonsQuery(p => exceptedIds.Contains(p.PersonId));
            var handler = Scope.ServiceProvider.GetRequiredService<IQueryHandler<GetPersonsQuery, PersonDto>>();

            // Act
            var result = await handler.RunQueryAsync(query);

            // Assert
            result.IsSucc.Should().BeTrue();
            result.IfSucc(rooms =>
                rooms.Should().BeEquivalentTo(excepted)
            );
        }
    }
}