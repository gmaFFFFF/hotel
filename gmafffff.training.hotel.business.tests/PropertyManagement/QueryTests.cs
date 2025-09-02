using FluentAssertions;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.PropertyManagement.Queries;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using JetBrains.Annotations;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PropertyManagement;

public partial class PropertyManagementTests {
    [TestSubject(typeof(GetRoomsQuery<>))]
    public class Query : TestContext {
        private readonly QueryHandlerFabric _queryHandlerFabric;

        public Query(ITestOutputHelper output) : base(output) {
            _queryHandlerFabric = Scope.ServiceProvider.GetRequiredService<QueryHandlerFabric>();
        }

        [Fact]
        public async Task CanQueryData() {
            // Arrange
            var query = new GetRoomsQuery<RoomDto>(Room<Guid>.WhereType(RoomType.Standard));
            var handler = _queryHandlerFabric.GetQueryHandler(query, default(RoomDto)!);
            var excepted = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.WhereType(RoomType.Standard).Compile())
                .Select(room => room.Adapt<RoomDto>(RoomDto.MapperConfig))
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