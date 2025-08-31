using System;
using System.Linq;
using System.Linq.Expressions;
using gmafffff.training.hotel.business.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.infrastructure.mapper.Required;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PropertyManagementMapperMapsterBusinessHotelBlock : IPropertyManagementMapperMapsterBusinessHotelBlock
    {
        public Expression<Func<HotelBlock<int, Guid>, HotelDto>> EntityToDto => p1 => new HotelDto(p1.Id, p1.Rooms.Select<Room<Guid>, RoomDto>(p2 => new RoomDto(p2.Id, p2.RoomDetails.Number, p2.RoomDetails.Type, p2.RoomDetails.Capacity) {RoomId = p2.Id}).ToList<RoomDto>())
        {
            BlockId = p1.Id,
            Rooms = p1.Rooms.Select<Room<Guid>, RoomDto>(p3 => new RoomDto(p3.Id, p3.RoomDetails.Number, p3.RoomDetails.Type, p3.RoomDetails.Capacity) {RoomId = p3.Id}).ToList<RoomDto>()
        };
    }
}