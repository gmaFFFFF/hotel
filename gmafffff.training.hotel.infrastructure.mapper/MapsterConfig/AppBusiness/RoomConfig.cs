using gmafffff.training.hotel.application.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig.AppBusiness;

public class RoomConfig : StandardMapsterConfig {
    public override void Register(TypeAdapterConfig config) {
        // RoomDto
        config.NewConfig<Room<Guid>, RoomDto>()
            .TwoWays()
            .Map(member: d => d.RoomId, source: s => s.Id)
            .Map(member: d => d, source: s => s.RoomDetails)
            .GenerateMapper(All);

        config.ForType<RoomDto, Room<Guid>>()
            .Ignore(d => d.Visit!, d => d.RoomCleanings)
            .GenerateMapper(Instance);
    }
}