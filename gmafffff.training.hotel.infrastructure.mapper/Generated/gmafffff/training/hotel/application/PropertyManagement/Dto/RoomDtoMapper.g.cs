using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.application.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.application.PropertyManagement.Dto
{
    public static partial class RoomDtoMapper
    {
        public static Expression<Func<RoomDto, Room<Guid>>> ProjectToGuidRoom => p1 => new Room<Guid>()
        {
            RoomDetails = p1 == null ? null : new RoomDetails(p1.Number, p1.Type, p1.Capacity) {},
            Id = p1.RoomId
        };
    }
}