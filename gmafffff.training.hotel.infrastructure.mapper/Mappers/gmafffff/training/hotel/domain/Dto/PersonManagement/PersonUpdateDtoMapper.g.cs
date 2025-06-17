using System;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.infrastructure.mapper.MapsterConfig;

namespace gmafffff.training.hotel.domain.Dto.PersonManagement
{
    public static partial class PersonUpdateDtoMapper
    {
        public static Person<Guid> AdaptTo(this PersonUpdateDto p1, Person<Guid> p2)
        {
            if (p1 == null)
            {
                return null;
            }
            Person<Guid> result = p2 ?? new Person<Guid>();
            
            result.FullName = funcMain1(p1, result.FullName);
            PersonConfig.SetInitialPersonHistory(result);
            return result;
            
        }
        
        private static PersonFullName funcMain1(PersonUpdateDto p3, PersonFullName p4)
        {
            if (p3 == null)
            {
                return null;
            }
            PersonFullName result = new PersonFullName(p3.SurName, p3.FirstName, p3.Patronymic) {};
            return result;
            
        }
    }
}