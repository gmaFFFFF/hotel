using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig;

public class RoomConfig : StandardMapsterConfig {
    public override void Register(TypeAdapterConfig config) {
        // RoomDto
        config.NewConfig<Room<Guid>, RoomDto>()
            .TwoWays()
            .Map(member: d => d.RoomId, source: s => s.Id)
            .Map(member: d => d, source: s => s.RoomDetails)
            .GenerateMapper(All);

        config.ForType<RoomDto, Room<Guid>>()
            .Ignore(d => d.Visit!)
            .GenerateMapper(Instance);

        // RoomUpdateDto
        config.NewConfig<RoomUpdateDto, Room<Guid>>()
            .Map(member: d => d.RoomDetails, source: s => s)
            .Ignore(d => d.Id, d => d.Visit!)
            .GenerateMapper(MapType.MapToTarget);

        // RoomAddDto
        config.NewConfig<RoomAddDto, Room<Guid>>()
            .Inherits<RoomUpdateDto, Room<Guid>>()
            .GenerateMapper(MapType.Map);
    }
}