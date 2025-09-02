using System.Linq.Expressions;
using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using MapsterMapper;

namespace gmafffff.training.hotel.business.tests.Fixtures;

public class RoomMapper : IEntityMapperForwardExpression<Room<Guid>, int, RoomDto> {
    public Expression<Func<Room<Guid>, RoomDto>> EntityToDto {
        get {
            var mapper = new Mapper(RoomDto.MapperConfig);
            return mapper.From(default(Room<Guid>)!)
                .CreateProjectionExpression<RoomDto>();
        }
    }
}