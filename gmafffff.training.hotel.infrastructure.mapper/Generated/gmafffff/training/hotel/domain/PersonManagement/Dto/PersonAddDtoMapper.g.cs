using System;
using gmafffff.training.hotel.domain.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;
using gmafffff.training.hotel.infrastructure.mapper.MapsterConfig.DomainBusiness;

namespace gmafffff.training.hotel.domain.PersonManagement.Dto
{
    public static partial class PersonAddDtoMapper
    {
        public static Person<Guid> AdaptToGuidPerson(this PersonAddDto p1)
        {
            if (p1 == null)
            {
                return null;
            }
            Person<Guid> result = new Person<Guid>();
            
            result.FullName = p1 == null ? null : new PersonFullName(p1.SurName, p1.FirstName, p1.Patronymic) {};
            PersonConfig.SetInitialPersonHistory(result);
            return result;
            
        }
    }
}