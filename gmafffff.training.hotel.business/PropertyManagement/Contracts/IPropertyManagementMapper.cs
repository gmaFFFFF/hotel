using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.business.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.business.PropertyManagement.Contracts;

public interface IPropertyManagementMapper :
    IEntityMapperForward<Room<Guid>, int, RoomDto>,
    IEntityMapperForwardExpression<Room<Guid>, int, RoomDto>,
    IEntityMapperForward<HotelBlock<int, Guid>, int, HotelDto>;