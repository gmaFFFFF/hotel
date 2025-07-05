using System;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.infrastructure.mapper.MapsterConfig;
using gmafffff.training.hotel.infrastructure.mapper.Required;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PersonManagementMapperMapsterDomain : IPersonManagementMapperMapsterDomain
    {
        public Person<Guid> Map(PersonAddDto p1)
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
        public Person<Guid> Update(PersonAddDto p2, Person<Guid> p3)
        {
            if (p2 == null)
            {
                return null;
            }
            Person<Guid> result = p3 ?? new Person<Guid>();
            
            result.FullName = funcMain1(p2, result.FullName);
            PersonConfig.SetInitialPersonHistory(result);
            return result;
            
        }
        public Person<Guid> Map(PersonUpdateDto p6)
        {
            if (p6 == null)
            {
                return null;
            }
            Person<Guid> result = new Person<Guid>();
            
            result.FullName = p6 == null ? null : new PersonFullName(p6.SurName, p6.FirstName, p6.Patronymic) {};
            PersonConfig.SetInitialPersonHistory(result);
            return result;
            
        }
        public Person<Guid> Update(PersonUpdateDto p7, Person<Guid> p8)
        {
            if (p7 == null)
            {
                return null;
            }
            Person<Guid> result = p8 ?? new Person<Guid>();
            
            result.FullName = funcMain2(p7, result.FullName);
            PersonConfig.SetInitialPersonHistory(result);
            return result;
            
        }
        
        private PersonFullName funcMain1(PersonAddDto p4, PersonFullName p5)
        {
            if (p4 == null)
            {
                return null;
            }
            PersonFullName result = new PersonFullName(p4.SurName, p4.FirstName, p4.Patronymic) {};
            return result;
            
        }
        
        private PersonFullName funcMain2(PersonUpdateDto p9, PersonFullName p10)
        {
            if (p9 == null)
            {
                return null;
            }
            PersonFullName result = new PersonFullName(p9.SurName, p9.FirstName, p9.Patronymic) {};
            return result;
            
        }
    }
}