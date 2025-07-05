using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;

namespace gmafffff.training.hotel.domain.Contracts.Mappers;

public interface IPropertyManagementMapper :
    IEntityMapperBackward<Room<Guid>, int, RoomAddDto>,
    IEntityMapperBackward<Room<Guid>, int, RoomUpdateDto>;