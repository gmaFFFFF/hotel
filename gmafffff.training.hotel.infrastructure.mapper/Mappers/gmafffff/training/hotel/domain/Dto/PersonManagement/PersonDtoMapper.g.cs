using System;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.domain.Dto.PersonManagement
{
    public static partial class PersonDtoMapper
    {
        public static Person<Guid> AdaptToGuidPerson(this PersonDto p1)
        {
            return p1 == null ? null : new Person<Guid>()
            {
                FullName = p1 == null ? null : new PersonFullName(p1.SurName, p1.FirstName, p1.Patronymic) {},
                Id = p1.PersonId
            };
        }
        public static Person<Guid> AdaptTo(this PersonDto p2, Person<Guid> p3)
        {
            if (p2 == null)
            {
                return null;
            }
            Person<Guid> result = p3 ?? new Person<Guid>();
            
            result.FullName = funcMain1(p2, result.FullName);
            result.Id = p2.PersonId;
            return result;
            
        }
        
        private static PersonFullName funcMain1(PersonDto p4, PersonFullName p5)
        {
            if (p4 == null)
            {
                return null;
            }
            PersonFullName result = new PersonFullName(p4.SurName, p4.FirstName, p4.Patronymic) {};
            return result;
            
        }
    }
}