using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.business.PersonManagement.Dto;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.domain.Model
{
    public static partial class GuidPersonMapper
    {
        public static PersonDto AdaptToPersonDto(this Person<Guid> p1)
        {
            return p1 == null ? null : new PersonDto(p1.Id, p1.FullName == null ? null : p1.FullName.SurName, p1.FullName == null ? null : p1.FullName.FirstName, p1.FullName == null ? null : p1.FullName.Patronymic)
            {
                PersonId = p1.Id,
                SurName = p1.FullName == null ? null : p1.FullName.SurName,
                FirstName = p1.FullName == null ? null : p1.FullName.FirstName,
                Patronymic = p1.FullName == null ? null : p1.FullName.Patronymic
            };
        }
        public static PersonDto AdaptTo(this Person<Guid> p2, PersonDto p3)
        {
            if (p2 == null)
            {
                return null;
            }
            PersonDto result = new PersonDto(p2.Id, p2.FullName == null ? null : p2.FullName.SurName, p2.FullName == null ? null : p2.FullName.FirstName, p2.FullName == null ? null : p2.FullName.Patronymic)
            {
                PersonId = p2.Id,
                SurName = p2.FullName == null ? null : p2.FullName.SurName,
                FirstName = p2.FullName == null ? null : p2.FullName.FirstName,
                Patronymic = p2.FullName == null ? null : p2.FullName.Patronymic
            };
            return result;
            
        }
        public static Expression<Func<Person<Guid>, PersonDto>> ProjectToPersonDto => p4 => new PersonDto(p4.Id, p4.FullName.SurName, p4.FullName.FirstName, p4.FullName.Patronymic) {PersonId = p4.Id};
    }
}