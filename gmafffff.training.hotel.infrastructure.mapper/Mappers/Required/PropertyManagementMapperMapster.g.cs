using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.infrastructure.mapper.Required;
using Mapster;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PropertyManagementMapperMapster : IPropertyManagementMapperMapster
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
        public Room<Guid> Map(RoomAddDto p13)
        {
            return p13 == null ? null : new Room<Guid>() {RoomDetails = (RoomUpdateDto)p13 == null ? null : new RoomDetails(((RoomUpdateDto)p13).Number, ((RoomUpdateDto)p13).Type, ((RoomUpdateDto)p13).Capacity) {}};
        }
        public Room<Guid> Update(RoomAddDto p14, Room<Guid> p15)
        {
            if (p14 == null)
            {
                return null;
            }
            Room<Guid> result = p15 ?? new Room<Guid>();
            
            result.RoomDetails = funcMain9((RoomUpdateDto)p14, result.RoomDetails);
            return result;
            
        }
        public Room<Guid> Map(RoomUpdateDto p18)
        {
            return p18 == null ? null : new Room<Guid>() {RoomDetails = p18 == null ? null : new RoomDetails(p18.Number, p18.Type, p18.Capacity) {}};
        }
        public Room<Guid> Update(RoomUpdateDto p19, Room<Guid> p20)
        {
            if (p19 == null)
            {
                return null;
            }
            Room<Guid> result = p20 ?? new Room<Guid>();
            
            result.RoomDetails = funcMain10(p19, result.RoomDetails);
            return result;
            
        }
        public HotelDto Map(HotelBlock<int, Guid> p23)
        {
            return p23 == null ? null : new HotelDto(p23.Id, funcMain11(p23.Rooms))
            {
                BlockId = p23.Id,
                Rooms = funcMain16(p23.Rooms)
            };
        }
        public HotelDto Update(HotelBlock<int, Guid> p34, HotelDto p35)
        {
            if (p34 == null)
            {
                return null;
            }
            HotelDto result = new HotelDto(p34.Id, funcMain21(p34.Rooms))
            {
                BlockId = p34.Id,
                Rooms = funcMain26(p34.Rooms)
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
        
        private RoomDetails funcMain9(RoomUpdateDto p16, RoomDetails p17)
        {
            if (p16 == null)
            {
                return null;
            }
            RoomDetails result = new RoomDetails(p16.Number, p16.Type, p16.Capacity) {};
            return result;
            
        }
        
        private RoomDetails funcMain10(RoomUpdateDto p21, RoomDetails p22)
        {
            if (p21 == null)
            {
                return null;
            }
            RoomDetails result = new RoomDetails(p21.Number, p21.Type, p21.Capacity) {};
            return result;
            
        }
        
        private ICollection<RoomDto> funcMain11(ICollection<Room<Guid>> p24)
        {
            if (p24 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p24.Count);
            
            IEnumerator<Room<Guid>> enumerator = p24.GetEnumerator();
            
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
        
        private ICollection<RoomDto> funcMain16(ICollection<Room<Guid>> p29)
        {
            if (p29 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p29.Count);
            
            IEnumerator<Room<Guid>> enumerator = p29.GetEnumerator();
            
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
        
        private ICollection<RoomDto> funcMain21(ICollection<Room<Guid>> p36)
        {
            if (p36 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p36.Count);
            
            IEnumerator<Room<Guid>> enumerator = p36.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Room<Guid> item = enumerator.Current;
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain22((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain23(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain24((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain25(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private ICollection<RoomDto> funcMain26(ICollection<Room<Guid>> p41)
        {
            if (p41 == null)
            {
                return null;
            }
            ICollection<RoomDto> result = new List<RoomDto>(p41.Count);
            
            IEnumerator<Room<Guid>> enumerator = p41.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Room<Guid> item = enumerator.Current;
                result.Add(item == null ? null : new RoomDto(item.Id, item.RoomDetails == null ? null : item.RoomDetails.Number, funcMain27((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)), funcMain28(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity))
                {
                    RoomId = item.Id,
                    Number = item.RoomDetails == null ? null : item.RoomDetails.Number,
                    Type = funcMain29((object)(item.RoomDetails == null ? null : (RoomType?)item.RoomDetails.Type)),
                    Capacity = funcMain30(item.RoomDetails == null ? null : (byte?)item.RoomDetails.Capacity)
                });
            }
            return result;
            
        }
        
        private RoomType funcMain12(object p25)
        {
            return p25 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p25.GetType()).Invoke(p25);
        }
        
        private byte funcMain13(byte? p26)
        {
            return p26 == null ? ((byte)0) : (byte)p26;
        }
        
        private RoomType funcMain14(object p27)
        {
            return p27 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p27.GetType()).Invoke(p27);
        }
        
        private byte funcMain15(byte? p28)
        {
            return p28 == null ? ((byte)0) : (byte)p28;
        }
        
        private RoomType funcMain17(object p30)
        {
            return p30 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p30.GetType()).Invoke(p30);
        }
        
        private byte funcMain18(byte? p31)
        {
            return p31 == null ? ((byte)0) : (byte)p31;
        }
        
        private RoomType funcMain19(object p32)
        {
            return p32 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p32.GetType()).Invoke(p32);
        }
        
        private byte funcMain20(byte? p33)
        {
            return p33 == null ? ((byte)0) : (byte)p33;
        }
        
        private RoomType funcMain22(object p37)
        {
            return p37 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p37.GetType()).Invoke(p37);
        }
        
        private byte funcMain23(byte? p38)
        {
            return p38 == null ? ((byte)0) : (byte)p38;
        }
        
        private RoomType funcMain24(object p39)
        {
            return p39 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p39.GetType()).Invoke(p39);
        }
        
        private byte funcMain25(byte? p40)
        {
            return p40 == null ? ((byte)0) : (byte)p40;
        }
        
        private RoomType funcMain27(object p42)
        {
            return p42 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p42.GetType()).Invoke(p42);
        }
        
        private byte funcMain28(byte? p43)
        {
            return p43 == null ? ((byte)0) : (byte)p43;
        }
        
        private RoomType funcMain29(object p44)
        {
            return p44 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p44.GetType()).Invoke(p44);
        }
        
        private byte funcMain30(byte? p45)
        {
            return p45 == null ? ((byte)0) : (byte)p45;
        }
    }
}