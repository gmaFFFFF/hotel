using FluentAssertions;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.PropertyManagement.Queries;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Model;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PropertyManagement;

public partial class PropertyManagementTests {
    public class Query(ITestOutputHelper output) : TestContext(output) {
        [Fact]
        public async Task CanQueryData() {
            // Arrange
            var query = new GetRoomsQuery(Room<Guid>.FilterByType(RoomType.Standard));
            var handler = Scope.ServiceProvider.GetRequiredService<IQueryHandler<GetRoomsQuery, RoomDto>>();
            var excepted = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.FilterByType(RoomType.Standard).Compile())
                .Select(room => room.Adapt<RoomDto>())
                .ToArray();

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