using System;
using gmafffff.training.hotel.domain.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.infrastructure.mapper.Required;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PropertyManagementMapperMapsterDomain : IPropertyManagementMapperMapsterDomain
    {
        public Room<Guid> Map(RoomAddDto p1)
        {
            return p1 == null ? null : new Room<Guid>() {RoomDetails = (RoomUpdateDto)p1 == null ? null : new RoomDetails(((RoomUpdateDto)p1).Number, ((RoomUpdateDto)p1).Type, ((RoomUpdateDto)p1).Capacity) {}};
        }
        public Room<Guid> Update(RoomAddDto p2, Room<Guid> p3)
        {
            if (p2 == null)
            {
                return null;
            }
            Room<Guid> result = p3 ?? new Room<Guid>();
            
            result.RoomDetails = funcMain1((RoomUpdateDto)p2, result.RoomDetails);
            return result;
            
        }
        public Room<Guid> Map(RoomUpdateDto p6)
        {
            return p6 == null ? null : new Room<Guid>() {RoomDetails = p6 == null ? null : new RoomDetails(p6.Number, p6.Type, p6.Capacity) {}};
        }
        public Room<Guid> Update(RoomUpdateDto p7, Room<Guid> p8)
        {
            if (p7 == null)
            {
                return null;
            }
            Room<Guid> result = p8 ?? new Room<Guid>();
            
            result.RoomDetails = funcMain2(p7, result.RoomDetails);
            return result;
            
        }
        
        private RoomDetails funcMain1(RoomUpdateDto p4, RoomDetails p5)
        {
            if (p4 == null)
            {
                return null;
            }
            RoomDetails result = new RoomDetails(p4.Number, p4.Type, p4.Capacity) {};
            return result;
            
        }
        
        private RoomDetails funcMain2(RoomUpdateDto p9, RoomDetails p10)
        {
            if (p9 == null)
            {
                return null;
            }
            RoomDetails result = new RoomDetails(p9.Number, p9.Type, p9.Capacity) {};
            return result;
            
        }
    }
}