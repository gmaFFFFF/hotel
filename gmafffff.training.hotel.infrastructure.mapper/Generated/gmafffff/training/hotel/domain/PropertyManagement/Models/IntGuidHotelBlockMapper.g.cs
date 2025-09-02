using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using gmafffff.training.hotel.application.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using Mapster;

namespace gmafffff.training.hotel.domain.PropertyManagement.Models
{
    public static partial class IntGuidHotelBlockMapper
    {
        public static HotelDto AdaptToHotelDto(this HotelBlock<int, Guid> p1)
        {
            return p1 == null ? null : new HotelDto(p1.Id, funcMain1(p1.Rooms))
            {
                BlockId = p1.Id,
                Rooms = funcMain6(p1.Rooms)
            };
        }
        public static HotelDto AdaptTo(this HotelBlock<int, Guid> p12, HotelDto p13)
        {
            if (p12 == null)
            {
                return null;
            }
            HotelDto result = new HotelDto(p12.Id, funcMain11(p12.Rooms))
            {
                BlockId = p12.Id,
                Rooms = funcMain16(p12.Rooms)
            };
            return result;
            
        }
        public static Expression<Func<HotelBlock<int, Guid>, HotelDto>> ProjectToHotelDto => p24 => new HotelDto(p24.Id, p24.Rooms.Select<Room<Guid>, RoomDto>(p25 => new RoomDto(p25.Id, p25.RoomDetails.Number, p25.RoomDetails.Type, p25.RoomDetails.Capacity) {RoomId = p25.Id}).ToList<RoomDto>())
        {
            BlockId = p24.Id,
            Rooms = p24.Rooms.Select<Room<Guid>, RoomDto>(p26 => new RoomDto(p26.Id, p26.RoomDetails.Number, p26.RoomDetails.Type, p26.RoomDetails.Capacity) {RoomId = p26.Id}).ToList<RoomDto>()
        };
        
        private static ICollection<RoomDto> funcMain1(ICollection<Room<Guid>> p2)
        {
            if (p2 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p2.Count);
            
            IEnumerator<Room<Guid>> enumerator = p2.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Room<Guid> item = enumerator.Current;
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain2((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain3(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain4((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain5(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private static ICollection<RoomDto> funcMain6(ICollection<Room<Guid>> p7)
        {
            if (p7 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p7.Count);
            
            IEnumerator<Room<Guid>> enumerator = p7.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Room<Guid> item = enumerator.Current;
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain7((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain8(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain9((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain10(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private static ICollection<RoomDto> funcMain11(ICollection<Room<Guid>> p14)
        {
            if (p14 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p14.Count);
            
            IEnumerator<Room<Guid>> enumerator = p14.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Room<Guid> item = enumerator.Current;
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain12((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain13(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain14((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain15(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private static ICollection<RoomDto> funcMain16(ICollection<Room<Guid>> p19)
        {
            if (p19 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p19.Count);
            
            IEnumerator<Room<Guid>> enumerator = p19.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Room<Guid> item = enumerator.Current;
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain17((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain18(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain19((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain20(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private static RoomType funcMain2(object p3)
        {
            return p3 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p3.GetType()).Invoke(p3);
        }
        
        private static byte funcMain3(byte? p4)
        {
            return p4 == null ? ((byte)0) : (byte)p4;
        }
        
        private static RoomType funcMain4(object p5)
        {
            return p5 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p5.GetType()).Invoke(p5);
        }
        
        private static byte funcMain5(byte? p6)
        {
            return p6 == null ? ((byte)0) : (byte)p6;
        }
        
        private static RoomType funcMain7(object p8)
        {
            return p8 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p8.GetType()).Invoke(p8);
        }
        
        private static byte funcMain8(byte? p9)
        {
            return p9 == null ? ((byte)0) : (byte)p9;
        }
        
        private static RoomType funcMain9(object p10)
        {
            return p10 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p10.GetType()).Invoke(p10);
        }
        
        private static byte funcMain10(byte? p11)
        {
            return p11 == null ? ((byte)0) : (byte)p11;
        }
        
        private static RoomType funcMain12(object p15)
        {
            return p15 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p15.GetType()).Invoke(p15);
        }
        
        private static byte funcMain13(byte? p16)
        {
            return p16 == null ? ((byte)0) : (byte)p16;
        }
        
        private static RoomType funcMain14(object p17)
        {
            return p17 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p17.GetType()).Invoke(p17);
        }
        
        private static byte funcMain15(byte? p18)
        {
            return p18 == null ? ((byte)0) : (byte)p18;
        }
        
        private static RoomType funcMain17(object p20)
        {
            return p20 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p20.GetType()).Invoke(p20);
        }
        
        private static byte funcMain18(byte? p21)
        {
            return p21 == null ? ((byte)0) : (byte)p21;
        }
        
        private static RoomType funcMain19(object p22)
        {
            return p22 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p22.GetType()).Invoke(p22);
        }
        
        private static byte funcMain20(byte? p23)
        {
            return p23 == null ? ((byte)0) : (byte)p23;
        }
    }
}