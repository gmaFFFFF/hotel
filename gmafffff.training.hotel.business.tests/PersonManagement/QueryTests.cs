using FluentAssertions;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.PersonManagement.Dto;
using gmafffff.training.hotel.business.PersonManagement.Handlers;
using gmafffff.training.hotel.business.PersonManagement.Queries;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using JetBrains.Annotations;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PersonManagement;

public partial class PersonManagementTests {
    [TestSubject(typeof(GetPersonsQueryHandler))]
    public class Query(ITestOutputHelper output) : TestContext(output) {
        [Fact]
        public async Task CanQueryData() {
            // Arrange
            var exceptedIds = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.WhereFreeRoom.Not().Compile())
                .SelectMany(r => r.Visit!.Visitors)
                .ToArray();
            var excepted = FakeHotel.Persons
                .Where(p => exceptedIds.Contains(p.Id))
                .Select(person => person.Adapt<PersonDto>())
                .ToArray();
            var query = new GetPersonsQuery<PersonDto>(p => exceptedIds.Contains(p.PersonId));
            var handler =
                Scope.ServiceProvider.GetRequiredService<IQueryHandler<GetPersonsQuery<PersonDto>, PersonDto>>();

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