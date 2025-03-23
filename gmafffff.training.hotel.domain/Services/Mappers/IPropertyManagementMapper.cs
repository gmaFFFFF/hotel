using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;

namespace gmafffff.training.hotel.domain.Services.Mappers;

public interface IPropertyManagementMapper :
    IEntityMapperForward<Room<Guid>, int, RoomDto>,
    IEntityMapperForwardExpression<Room<Guid>, int, RoomDto>,
    IEntityMapperBackward<Room<Guid>, int, RoomAddDto>,
    IEntityMapperBackward<Room<Guid>, int, RoomUpdateDto>,
    IEntityMapperForward<HotelBlock<int, Guid>, int, HotelDto>;