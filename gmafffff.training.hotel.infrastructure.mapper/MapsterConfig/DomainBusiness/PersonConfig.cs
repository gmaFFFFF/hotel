using gmafffff.training.hotel.domain.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;
using gmafffff.training.hotel.domain.SettlementManagement.Models;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig.DomainBusiness;

public class PersonConfig : StandardMapsterConfig {
    public override void Register(TypeAdapterConfig config) {
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
        person.History ??= new VisitorHistory(Count: 0, Duration: 0);
    }
}