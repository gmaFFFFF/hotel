using System;
using gmafffff.training.hotel.domain.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.domain.PropertyManagement.Dto
{
    public static partial class RoomAddDtoMapper
    {
        public static Room<Guid> AdaptToGuidRoom(this RoomAddDto p1)
        {
            return p1 == null ? null : new Room<Guid>() {RoomDetails = (RoomUpdateDto)p1 == null ? null : new RoomDetails(((RoomUpdateDto)p1).Number, ((RoomUpdateDto)p1).Type, ((RoomUpdateDto)p1).Capacity) {}};
        }
    }
}