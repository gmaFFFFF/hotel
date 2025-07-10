using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.business.PersonManagement.Dto;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig.DomainBusiness;

public class PersonConfig : StandardMapsterConfig {
    public override void Register(TypeAdapterConfig config) {
        // PersonDto
        config.NewConfig<Person<Guid>, PersonDto>()
            .TwoWays()
            .Map(member: d => d.PersonId, source: s => s.Id)
            .Map(member: d => d, source: s => s.FullName)
            .GenerateMapper(All);

        config.ForType<PersonDto, Person<Guid>>()
            .Ignore(d => d.History)
            .GenerateMapper(Instance);

        // PersonAddDto
        config.ForType<PersonAddDto, Person<Guid>>()
            .Map(member: d => d.FullName, source: s => s)
            .Ignore(d => d.Id, d => d.History)
            .AfterMappingInline(person => SetInitialPersonHistory(person))
            .GenerateMapper(MapType.Map);

        // PersonUpdateDto
        config.ForType<PersonUpdateDto, Person<Guid>>()
            .Map(member: d => d.FullName, source: s => s)
            .Ignore(d => d.Id, d => d.History)
            .GenerateMapper(MapType.MapToTarget);
    }

    public static void SetInitialPersonHistory(Person<Guid> person) {
        person.History ??= new VisitorHistory(0, 0);
    }
}