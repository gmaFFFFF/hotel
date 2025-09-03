using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.application.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.domain.PropertyManagement.Models
{
    public static partial class GuidRoomMapper
    {
        public static Expression<Func<Room<Guid>, RoomDto>> ProjectToRoomDto => p1 => new RoomDto(p1.Id, p1.RoomDetails.Number, p1.RoomDetails.Type, p1.RoomDetails.Capacity) {RoomId = p1.Id};
    }
}