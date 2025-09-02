using gmafffff.training.hotel.domain.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig.DomainBusiness;

public class RoomConfig : StandardMapsterConfig {
    public override void Register(TypeAdapterConfig config) {
        // RoomUpdateDto
        config.NewConfig<RoomUpdateDto, Room<Guid>>()
            .Map(member: d => d.RoomDetails, source: s => s)
            .Ignore(d => d.Id, d => d.Visit!, d => d.RoomCleanings)
            .GenerateMapper(MapType.MapToTarget);

        // RoomAddDto
        config.NewConfig<RoomAddDto, Room<Guid>>()
            .Inherits<RoomUpdateDto, Room<Guid>>()
            .GenerateMapper(MapType.Map);
    }
}