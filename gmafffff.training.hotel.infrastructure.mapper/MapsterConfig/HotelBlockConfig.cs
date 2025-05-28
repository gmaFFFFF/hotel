using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.infrastructure.mapper.MapsterConfig;

public class HotelBlockConfig : StandardMapsterConfig {
    public override void Register(TypeAdapterConfig config) {
        // HotelDto
        config
            .NewConfig<HotelBlock<int, Guid>, HotelDto>()
            .TwoWays()
            .Map(member: d => d.BlockId, source: s => s.Id)
            .GenerateMapper(All);
    }
}