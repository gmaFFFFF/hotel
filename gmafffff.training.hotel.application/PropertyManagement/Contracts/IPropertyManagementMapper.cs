using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.application.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.application.PropertyManagement.Contracts;

public interface IPropertyManagementMapperRoom :
    IEntityMapperForward<Room<Guid>, int, RoomDto>,
    IEntityMapperForwardExpression<Room<Guid>, int, RoomDto>;

public interface IPropertyManagementMapperHotelBlock :
    IEntityMapperForwardExpression<HotelBlock<int, Guid>, int, HotelDto>;