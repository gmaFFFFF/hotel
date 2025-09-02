using System.Linq.Expressions;
using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.PersonManagement.Models;
using MapsterMapper;

namespace gmafffff.training.hotel.business.tests.Fixtures;

public class PersonMapper : IEntityMapperForwardExpression<Person<Guid>, Guid, PersonDto> {
    public Expression<Func<Person<Guid>, PersonDto>> EntityToDto {
        get {
            var mapper = new Mapper(PersonDto.MapperConfig);
            return mapper.From(default(Person<Guid>)!)
                .CreateProjectionExpression<PersonDto>();
        }
    }
}