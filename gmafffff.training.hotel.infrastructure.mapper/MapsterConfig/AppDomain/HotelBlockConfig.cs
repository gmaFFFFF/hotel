using gmafffff.training.hotel.application.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig.AppDomain;

public class HotelBlockConfig : StandardMapsterConfig {
    public override void Register(TypeAdapterConfig config) {
        // HotelDto
        config
            .NewConfig<HotelBlock<int, Guid>, HotelDto>()
            .TwoWays()
            .Map(member: d => d.BlockId, source: s => s.Id)
            .GenerateMapper(MapType.Projection);
    }
}