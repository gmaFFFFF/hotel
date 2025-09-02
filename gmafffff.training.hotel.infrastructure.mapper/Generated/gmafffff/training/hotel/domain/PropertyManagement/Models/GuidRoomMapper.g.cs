using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.application.PropertyManagement.Dto;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using Mapster;

namespace gmafffff.training.hotel.domain.PropertyManagement.Models
{
    public static partial class GuidRoomMapper
    {
        public static RoomDto AdaptToRoomDto(this Room<Guid> p1)
        {
            return p1 == null ? null : new RoomDto(p1.Id, p1.RoomDetails == null ? null : p1.RoomDetails.Number, funcMain1((object)(p1.RoomDetails == null ? null : (RoomType?)p1.RoomDetails.Type)), funcMain2(p1.RoomDetails == null ? null : (byte?)p1.RoomDetails.Capacity))
            {
                RoomId = p1.Id,
                Number = p1.RoomDetails == null ? null : p1.RoomDetails.Number,
                Type = funcMain3((object)(p1.RoomDetails == null ? null : (RoomType?)p1.RoomDetails.Type)),
                Capacity = funcMain4(p1.RoomDetails == null ? null : (byte?)p1.RoomDetails.Capacity)
            };
        }
        public static RoomDto AdaptTo(this Room<Guid> p6, RoomDto p7)
        {
            if (p6 == null)
            {
                return null;
            }
            RoomDto result = new RoomDto(p6.Id, p6.RoomDetails == null ? null : p6.RoomDetails.Number, funcMain5((object)(p6.RoomDetails == null ? null : (RoomType?)p6.RoomDetails.Type)), funcMain6(p6.RoomDetails == null ? null : (byte?)p6.RoomDetails.Capacity))
            {
                RoomId = p6.Id,
                Number = p6.RoomDetails == null ? null : p6.RoomDetails.Number,
                Type = funcMain7((object)(p6.RoomDetails == null ? null : (RoomType?)p6.RoomDetails.Type)),
                Capacity = funcMain8(p6.RoomDetails == null ? null : (byte?)p6.RoomDetails.Capacity)
            };
            return result;
            
        }
        public static Expression<Func<Room<Guid>, RoomDto>> ProjectToRoomDto => p12 => new RoomDto(p12.Id, p12.RoomDetails.Number, p12.RoomDetails.Type, p12.RoomDetails.Capacity) {RoomId = p12.Id};
        
        private static RoomType funcMain1(object p2)
        {
            return p2 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p2.GetType()).Invoke(p2);
        }
        
        private static byte funcMain2(byte? p3)
        {
            return p3 == null ? ((byte)0) : (byte)p3;
        }
        
        private static RoomType funcMain3(object p4)
        {
            return p4 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p4.GetType()).Invoke(p4);
        }
        
        private static byte funcMain4(byte? p5)
        {
            return p5 == null ? ((byte)0) : (byte)p5;
        }
        
        private static RoomType funcMain5(object p8)
        {
            return p8 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p8.GetType()).Invoke(p8);
        }
        
        private static byte funcMain6(byte? p9)
        {
            return p9 == null ? ((byte)0) : (byte)p9;
        }
        
        private static RoomType funcMain7(object p10)
        {
            return p10 == null ? RoomType.None : TypeAdapterConfig.GlobalSettings.GetDynamicMapFunction<RoomType>(p10.GetType()).Invoke(p10);
        }
        
        private static byte funcMain8(byte? p11)
        {
            return p11 == null ? ((byte)0) : (byte)p11;
        }
    }
}