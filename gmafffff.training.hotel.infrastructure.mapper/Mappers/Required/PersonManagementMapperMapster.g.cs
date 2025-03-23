using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.infrastructure.mapper.Required;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PersonManagementMapperMapster : IPersonManagementMapperMapster
    {
        public Expression<Func<Person<Guid>, PersonDto>> EntityToDto => p1 => new PersonDto(p1.Id, p1.FullName.SurName, p1.FullName.FirstName, p1.FullName.Patronymic) {PersonId = p1.Id};
        public PersonDto Map(Person<Guid> p2)
        {
            return p2 == null ? null : new PersonDto(p2.Id, p2.FullName == null ? null : p2.FullName.SurName, p2.FullName == null ? null : p2.FullName.FirstName, p2.FullName == null ? null : p2.FullName.Patronymic)
            {
                PersonId = p2.Id,
                SurName = p2.FullName == null ? null : p2.FullName.SurName,
                FirstName = p2.FullName == null ? null : p2.FullName.FirstName,
                Patronymic = p2.FullName == null ? null : p2.FullName.Patronymic
            };
        }
        public PersonDto Update(Person<Guid> p3, PersonDto p4)
        {
            if (p3 == null)
            {
                return null;
            }
            PersonDto result = new PersonDto(p3.Id, p3.FullName == null ? null : p3.FullName.SurName, p3.FullName == null ? null : p3.FullName.FirstName, p3.FullName == null ? null : p3.FullName.Patronymic)
            {
                PersonId = p3.Id,
                SurName = p3.FullName == null ? null : p3.FullName.SurName,
                FirstName = p3.FullName == null ? null : p3.FullName.FirstName,
                Patronymic = p3.FullName == null ? null : p3.FullName.Patronymic
            };
            return result;
            
        }
        public Person<Guid> Map(PersonAddDto p5)
        {
            return p5 == null ? null : new Person<Guid>() {FullName = p5 == null ? null : new PersonFullName(p5.SurName, p5.FirstName, p5.Patronymic) {}};
        }
        public Person<Guid> Update(PersonAddDto p6, Person<Guid> p7)
        {
            if (p6 == null)
            {
                return null;
            }
            Person<Guid> result = p7 ?? new Person<Guid>();
            
            result.FullName = funcMain1(p6, result.FullName);
            return result;
            
        }
        public Person<Guid> Map(PersonUpdateDto p10)
        {
            return p10 == null ? null : new Person<Guid>() {FullName = (PersonAddDto)p10 == null ? null : new PersonFullName(((PersonAddDto)p10).SurName, ((PersonAddDto)p10).FirstName, ((PersonAddDto)p10).Patronymic) {}};
        }
        public Person<Guid> Update(PersonUpdateDto p11, Person<Guid> p12)
        {
            if (p11 == null)
            {
                return null;
            }
            Person<Guid> result = p12 ?? new Person<Guid>();
            
            result.FullName = funcMain2((PersonAddDto)p11, result.FullName);
            return result;
            
        }
        
        private PersonFullName funcMain1(PersonAddDto p8, PersonFullName p9)
        {
            if (p8 == null)
            {
                return null;
            }
            PersonFullName result = new PersonFullName(p8.SurName, p8.FirstName, p8.Patronymic) {};
            return result;
            
        }
        
        private PersonFullName funcMain2(PersonAddDto p13, PersonFullName p14)
        {
            if (p13 == null)
            {
                return null;
            }
            PersonFullName result = new PersonFullName(p13.SurName, p13.FirstName, p13.Patronymic) {};
            return result;
            
        }
    }
}