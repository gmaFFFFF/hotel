using System;
using System.Linq;
using System.Linq.Expressions;
using gmafffff.training.hotel.application.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.application.PropertyManagement.Dto
{
    public static partial class HotelDtoMapper
    {
        public static Expression<Func<HotelDto, HotelBlock<int, Guid>>> ProjectToIntGuidHotelBlock => p1 => new HotelBlock<int, Guid>()
        {
            Rooms = p1.Rooms.Select<RoomDto, Room<Guid>>(p2 => new Room<Guid>()
            {
                RoomDetails = p2 == null ? null : new RoomDetails(p2.Number, p2.Type, p2.Capacity) {},
                Id = p2.RoomId
            }).ToList<Room<Guid>>(),
            Id = p1.BlockId
        };
    }
}