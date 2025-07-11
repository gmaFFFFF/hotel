using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using gmafffff.training.hotel.business.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.infrastructure.mapper.Required;
using Mapster;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PropertyManagementMapperMapsterBusiness : IPropertyManagementMapperMapsterBusiness
    {
        public Expression<Func<Room<Guid>, RoomDto>> EntityToDto => p1 => new RoomDto(p1.Id, p1.RoomDetails.Number, p1.RoomDetails.Type, p1.RoomDetails.Capacity) {RoomId = p1.Id};
        public RoomDto Map(Room<Guid> p2)
        {
            return p2 == null ? null : new RoomDto(p2.Id, p2.RoomDetails == null ? null : p2.RoomDetails.Number, funcMain1((object)(p2.RoomDetails == null ? null : (RoomType?)p2.RoomDetails.Type)), funcMain2(p2.RoomDetails == null ? null : (byte?)p2.RoomDetails.Capacity))
            {
                RoomId = p2.Id,
                Number = p2.RoomDetails == null ? null : p2.RoomDetails.Number,
                Type = funcMain3((object)(p2.RoomDetails == null ? null : (RoomType?)p2.RoomDetails.Type)),
                Capacity = funcMain4(p2.RoomDetails == null ? null : (byte?)p2.RoomDetails.Capacity)
            };
        }
        public RoomDto Update(Room<Guid> p7, RoomDto p8)
        {
            if (p7 == null)
            {
                return null;
            }
            RoomDto result = new RoomDto(p7.Id, p7.RoomDetails == null ? null : p7.RoomDetails.Number, funcMain5((object)(p7.RoomDetails == null ? null : (RoomType?)p7.RoomDetails.Type)), funcMain6(p7.RoomDetails == null ? null : (byte?)p7.RoomDetails.Capacity))
            {
                RoomId = p7.Id,
                Number = p7.RoomDetails == null ? null : p7.RoomDetails.Number,
                Type = funcMain7((object)(p7.RoomDetails == null ? null : (RoomType?)p7.RoomDetails.Type)),
                Capacity = funcMain8(p7.RoomDetails == null ? null : (byte?)p7.RoomDetails.Capacity)
            };
            return result;
            
        }
        public HotelDto Map(HotelBlock<int, Guid> p13)
        {
            return p13 == null ? null : new HotelDto(p13.Id, funcMain9(p13.Rooms))
            {
                BlockId = p13.Id,
                Rooms = funcMain14(p13.Rooms)
            };
        }
        public HotelDto Update(HotelBlock<int, Guid> p24, HotelDto p25)
        {
            if (p24 == null)
            {
                return null;
            }
            HotelDto result = new HotelDto(p24.Id, funcMain19(p24.Rooms))
            {
                BlockId = p24.Id,
                Rooms = funcMain24(p24.Rooms)
            };
            return result;
            
        }
        
        private RoomType funcMain1(object p3)
        {
            return p3 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p3.GetType()).Invoke(p3);
        }
        
        private byte funcMain2(byte? p4)
        {
            return p4 == null ? ((byte)0) : (byte)p4;
        }
        
        private RoomType funcMain3(object p5)
        {
            return p5 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p5.GetType()).Invoke(p5);
        }
        
        private byte funcMain4(byte? p6)
        {
            return p6 == null ? ((byte)0) : (byte)p6;
        }
        
        private RoomType funcMain5(object p9)
        {
            return p9 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p9.GetType()).Invoke(p9);
        }
        
        private byte funcMain6(byte? p10)
        {
            return p10 == null ? ((byte)0) : (byte)p10;
        }
        
        private RoomType funcMain7(object p11)
        {
            return p11 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p11.GetType()).Invoke(p11);
        }
        
        private byte funcMain8(byte? p12)
        {
            return p12 == null ? ((byte)0) : (byte)p12;
        }
        
        private ICollection<RoomDto> funcMain9(ICollection<Room<Guid>> p14)
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
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain10((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain11(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain12((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain13(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private ICollection<RoomDto> funcMain14(ICollection<Room<Guid>> p19)
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
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain15((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain16(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain17((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain18(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private ICollection<RoomDto> funcMain19(ICollection<Room<Guid>> p26)
        {
            if (p26 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p26.Count);
            
            IEnumerator<Room<Guid>> enumerator = p26.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Room<Guid> item = enumerator.Current;
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain20((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain21(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain22((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain23(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private ICollection<RoomDto> funcMain24(ICollection<Room<Guid>> p31)
        {
            if (p31 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p31.Count);
            
            IEnumerator<Room<Guid>> enumerator = p31.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Room<Guid> item = enumerator.Current;
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain25((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain26(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain27((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain28(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private RoomType funcMain10(object p15)
        {
            return p15 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p15.GetType()).Invoke(p15);
        }
        
        private byte funcMain11(byte? p16)
        {
            return p16 == null ? ((byte)0) : (byte)p16;
        }
        
        private RoomType funcMain12(object p17)
        {
            return p17 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p17.GetType()).Invoke(p17);
        }
        
        private byte funcMain13(byte? p18)
        {
            return p18 == null ? ((byte)0) : (byte)p18;
        }
        
        private RoomType funcMain15(object p20)
        {
            return p20 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p20.GetType()).Invoke(p20);
        }
        
        private byte funcMain16(byte? p21)
        {
            return p21 == null ? ((byte)0) : (byte)p21;
        }
        
        private RoomType funcMain17(object p22)
        {
            return p22 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p22.GetType()).Invoke(p22);
        }
        
        private byte funcMain18(byte? p23)
        {
            return p23 == null ? ((byte)0) : (byte)p23;
        }
        
        private RoomType funcMain20(object p27)
        {
            return p27 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p27.GetType()).Invoke(p27);
        }
        
        private byte funcMain21(byte? p28)
        {
            return p28 == null ? ((byte)0) : (byte)p28;
        }
        
        private RoomType funcMain22(object p29)
        {
            return p29 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p29.GetType()).Invoke(p29);
        }
        
        private byte funcMain23(byte? p30)
        {
            return p30 == null ? ((byte)0) : (byte)p30;
        }
        
        private RoomType funcMain25(object p32)
        {
            return p32 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p32.GetType()).Invoke(p32);
        }
        
        private byte funcMain26(byte? p33)
        {
            return p33 == null ? ((byte)0) : (byte)p33;
        }
        
        private RoomType funcMain27(object p34)
        {
            return p34 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p34.GetType()).Invoke(p34);
        }
        
        private byte funcMain28(byte? p35)
        {
            return p35 == null ? ((byte)0) : (byte)p35;
        }
    }
}