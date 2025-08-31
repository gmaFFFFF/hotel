using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.business.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using gmafffff.training.hotel.infrastructure.mapper.Required;
using Mapster;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PropertyManagementMapperMapsterBusinessRoom : IPropertyManagementMapperMapsterBusinessRoom
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
    }
}