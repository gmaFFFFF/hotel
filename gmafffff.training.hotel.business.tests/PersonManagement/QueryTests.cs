using FluentAssertions;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.PersonManagement.Handlers;
using gmafffff.training.hotel.business.PersonManagement.Queries;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.PersonManagement.Models;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using JetBrains.Annotations;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.PersonManagement;

public partial class PersonManagementTests {
    [TestSubject(typeof(GetPersonsQueryByDtoHandler<>))]
    [TestSubject(typeof(GetPersonsQueryByEntityHandler<>))]
    public class Query : TestContext {
        private readonly QueryHandlerFabric _queryHandlerFabric;

        public Query(ITestOutputHelper output) : base(output) {
            _queryHandlerFabric = Scope.ServiceProvider.GetRequiredService<QueryHandlerFabric>();
        }

        [Fact]
        public async Task QueryPersons_ByDtoFilter() {
            // Arrange
            var exceptedIds = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.WhereFreeRoom.Not().Compile())
                .SelectMany(r => r.Visit!.Visitors)
                .ToArray();
            var excepted = FakeHotel.Persons
                .Where(p => exceptedIds.Contains(p.Id))
                .Select(person => person.Adapt<PersonDto>(PersonDto.MapperConfig))
                .ToArray();
            var query = new GetPersonsQueryByDto<PersonDto>(p => exceptedIds.Contains(p.PersonId));
            var handler = _queryHandlerFabric.GetQueryHandler(query, default(PersonDto)!);

            // Act
            var result = await handler.RunQueryAsync(query);

            // Assert
            result.IsSucc.Should().BeTrue();
            result.IfSucc(persons =>
                persons.Should().BeEquivalentTo(excepted)
            );
        }

        [Fact]
        public async Task QueryPersons_ByEntityFullName() {
            // Arrange
            const byte lettersCount = 3;
            var sample = FakeHotel.Persons
                .SelectMany(p => new[] {
                    (p.Id, p.FullName.SurName), (p.Id, p.FullName.FirstName), (p.Id, p.FullName.Patronymic)
                })
                .Select(w => (w.Id, firstLetters: w.Item2?[..lettersCount]))
                .GroupBy(keySelector: pair => pair.firstLetters,
                    elementSelector: pair => pair, resultSelector: (s, pair) => (itemCount: pair.Count(), pair))
                .OrderBy(g => g.itemCount)
                .First()
                .pair;
            var firstLetters = sample.First().firstLetters;
            var excepted = sample.Select(s => s.Id).ToArray();


            var query = new GetPersonsQueryByEntity<PersonDto>(new Person<Guid>.FilterByFullName(firstLetters));
            var handler = _queryHandlerFabric.GetQueryHandler(query, default(PersonDto)!);

            // Act
            var result = await handler.RunQueryAsync(query);

            // Assert
            result.IsSucc.Should().BeTrue();
            result.IfSucc(persons =>
                persons.Select(p => p.PersonId).Should().BeEquivalentTo(excepted)
            );
        }
    }
}