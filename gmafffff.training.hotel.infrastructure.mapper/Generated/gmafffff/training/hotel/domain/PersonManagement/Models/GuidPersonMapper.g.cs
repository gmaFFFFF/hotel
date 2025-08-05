using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.business.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.domain.PersonManagement.Models
{
    public static partial class GuidPersonMapper
    {
        public static PersonFindDto AdaptToPersonFindDto(this Person<Guid> p1)
        {
            return p1 == null ? null : new PersonFindDto(p1.Id, p1.FullName.FirstName + (object)' ' + p1.FullName.SurName + (object)' ' + (p1.FullName.Patronymic ?? ""))
            {
                PersonId = p1.Id,
                FullName = p1.FullName.FirstName + (object)' ' + p1.FullName.SurName + (object)' ' + (p1.FullName.Patronymic ?? "")
            };
        }
        public static PersonFindDto AdaptTo(this Person<Guid> p2, PersonFindDto p3)
        {
            if (p2 == null)
            {
                return null;
            }
            PersonFindDto result = new PersonFindDto(p2.Id, p2.FullName.FirstName + (object)' ' + p2.FullName.SurName + (object)' ' + (p2.FullName.Patronymic ?? ""))
            {
                PersonId = p2.Id,
                FullName = p2.FullName.FirstName + (object)' ' + p2.FullName.SurName + (object)' ' + (p2.FullName.Patronymic ?? "")
            };
            return result;
            
        }
        public static PersonDto AdaptToPersonDto(this Person<Guid> p4)
        {
            return p4 == null ? null : new PersonDto(p4.Id, p4.FullName == null ? null : p4.FullName.SurName, p4.FullName == null ? null : p4.FullName.FirstName, p4.FullName == null ? null : p4.FullName.Patronymic)
            {
                PersonId = p4.Id,
                SurName = p4.FullName == null ? null : p4.FullName.SurName,
                FirstName = p4.FullName == null ? null : p4.FullName.FirstName,
                Patronymic = p4.FullName == null ? null : p4.FullName.Patronymic
            };
        }
        public static PersonDto AdaptTo(this Person<Guid> p5, PersonDto p6)
        {
            if (p5 == null)
            {
                return null;
            }
            PersonDto result = new PersonDto(p5.Id, p5.FullName == null ? null : p5.FullName.SurName, p5.FullName == null ? null : p5.FullName.FirstName, p5.FullName == null ? null : p5.FullName.Patronymic)
            {
                PersonId = p5.Id,
                SurName = p5.FullName == null ? null : p5.FullName.SurName,
                FirstName = p5.FullName == null ? null : p5.FullName.FirstName,
                Patronymic = p5.FullName == null ? null : p5.FullName.Patronymic
            };
            return result;
            
        }
        public static Expression<Func<Person<Guid>, PersonDto>> ProjectToPersonDto => p7 => new PersonDto(p7.Id, p7.FullName.SurName, p7.FullName.FirstName, p7.FullName.Patronymic) {PersonId = p7.Id};
    }
}