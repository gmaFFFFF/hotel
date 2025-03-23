using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig;

public class PersonConfig : StandardMapsterConfig {
    protected override void Configure(TypeAdapterConfig config) {
        // PersonDto
        config.NewConfig<Person<Guid>, PersonDto>()
            .TwoWays()
            .Map(member: d => d.PersonId, source: s => s.Id)
            .Map(member: d => d, source: s => s.FullName)
            .GenerateMapper(All);

        config.ForType<PersonDto, Person<Guid>>()
            .GenerateMapper(Instance);

        // PersonAddDto
        config.ForType<PersonAddDto, Person<Guid>>()
            .Map(member: d => d.FullName, source: s => s)
            .Ignore(d => d.Id)
            .GenerateMapper(MapType.Map);

        // PersonUpdateDto
        config.ForType<PersonUpdateDto, Person<Guid>>()
            .Inherits<PersonAddDto, Person<Guid>>()
            .GenerateMapper(MapType.MapToTarget);
    }
}