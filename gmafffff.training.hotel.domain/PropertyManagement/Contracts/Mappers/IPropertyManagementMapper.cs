using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.PropertyManagement.Dto;

namespace gmafffff.training.hotel.domain.PropertyManagement.Contracts.Mappers;

public interface IPropertyManagementMapper :
    IEntityMapperBackward<Room<Guid>, int, RoomAddDto>,
    IEntityMapperBackward<Room<Guid>, int, RoomUpdateDto>;