using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.domain.Dto.PropertyManagement
{
    public static partial class HotelDtoMapper
    {
        public static HotelBlock<int, Guid> AdaptToIntGuidHotelBlock(this HotelDto p1)
        {
            return p1 == null ? null : new HotelBlock<int, Guid>()
            {
                Rooms = funcMain1(p1.Rooms),
                Id = p1.BlockId
            };
        }
        public static HotelBlock<int, Guid> AdaptTo(this HotelDto p3, HotelBlock<int, Guid> p4)
        {
            if (p3 == null)
            {
                return null;
            }
            HotelBlock<int, Guid> result = p4 ?? new HotelBlock<int, Guid>();
            
            result.Rooms = funcMain2(p3.Rooms, result.Rooms);
            result.Id = p3.BlockId;
            return result;
            
        }
        public static Expression<Func<HotelDto, HotelBlock<int, Guid>>> ProjectToIntGuidHotelBlock => p7 => new HotelBlock<int, Guid>()
        {
            Rooms = p7.Rooms.Select<RoomDto, Room<Guid>>(p8 => new Room<Guid>()
            {
                RoomDetails = p8 == null ? null : new RoomDetails(p8.Number, p8.Type, p8.Capacity) {},
                Id = p8.RoomId
            }).ToList<Room<Guid>>(),
            Id = p7.BlockId
        };
        
        private static ICollection<Room<Guid>> funcMain1(ICollection<RoomDto> p2)
        {
            if (p2 == null)
            {
                return null;
            }
            ICollection<Room<Guid>> result = new List<Room<Guid>>(p2.Count);
            
            IEnumerator<RoomDto> enumerator = p2.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                RoomDto item = enumerator.Current;
                result.Add(item == null ? null : new Room<Guid>()
                {
                    RoomDetails = item == null ? null : new RoomDetails(item.Number, item.Type, item.Capacity) {},
                    Id = item.RoomId
                });
            }
            return result;
            
        }
        
        private static ICollection<Room<Guid>> funcMain2(ICollection<RoomDto> p5, ICollection<Room<Guid>> p6)
        {
            if (p5 == null)
            {
                return null;
            }
            ICollection<Room<Guid>> result = new List<Room<Guid>>(p5.Count);
            
            IEnumerator<RoomDto> enumerator = p5.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                RoomDto item = enumerator.Current;
                result.Add(item == null ? null : new Room<Guid>()
                {
                    RoomDetails = item == null ? null : new RoomDetails(item.Number, item.Type, item.Capacity) {},
                    Id = item.RoomId
                });
            }
            return result;
            
        }
    }
}