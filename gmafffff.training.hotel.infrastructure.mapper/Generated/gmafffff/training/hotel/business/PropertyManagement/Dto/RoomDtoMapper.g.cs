using System;
using gmafffff.training.hotel.business.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.business.PropertyManagement.Dto
{
    public static partial class RoomDtoMapper
    {
        public static Room<Guid> AdaptToGuidRoom(this RoomDto p1)
        {
            return p1 == null ? null : new Room<Guid>()
            {
                RoomDetails = p1 == null ? null : new RoomDetails(p1.Number, p1.Type, p1.Capacity) {},
                Id = p1.RoomId
            };
        }
        public static Room<Guid> AdaptTo(this RoomDto p2, Room<Guid> p3)
        {
            if (p2 == null)
            {
                return null;
            }
            Room<Guid> result = p3 ?? new Room<Guid>();
            
            result.RoomDetails = funcMain1(p2, result.RoomDetails);
            result.Id = p2.RoomId;
            return result;
            
        }
        
        private static RoomDetails funcMain1(RoomDto p4, RoomDetails p5)
        {
            if (p4 == null)
            {
                return null;
            }
            RoomDetails result = new RoomDetails(p4.Number, p4.Type, p4.Capacity) {};
            return result;
            
        }
    }
}