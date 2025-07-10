using System;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.domain.Dto.PropertyManagement
{
    public static partial class RoomUpdateDtoMapper
    {
        public static Room<Guid> AdaptTo(this RoomUpdateDto p1, Room<Guid> p2)
        {
            if (p1 == null)
            {
                return null;
            }
            Room<Guid> result = p2 ?? new Room<Guid>();
            
            result.RoomDetails = funcMain1(p1, result.RoomDetails);
            return result;
            
        }
        
        private static RoomDetails funcMain1(RoomUpdateDto p3, RoomDetails p4)
        {
            if (p3 == null)
            {
                return null;
            }
            RoomDetails result = new RoomDetails(p3.Number, p3.Type, p3.Capacity) {};
            return result;
            
        }
    }
}