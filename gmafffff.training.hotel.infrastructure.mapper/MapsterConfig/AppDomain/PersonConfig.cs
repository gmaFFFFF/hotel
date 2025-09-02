using gmafffff.training.hotel.application.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig.AppDomain;

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
    }
}