using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.application.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.application.PersonManagement.Dto
{
    public static partial class PersonDtoMapper
    {
        public static Expression<Func<PersonDto, Person<Guid>>> ProjectToGuidPerson => p1 => new Person<Guid>()
        {
            FullName = p1 == null ? null : new PersonFullName(p1.SurName, p1.FirstName, p1.Patronymic) {},
            Id = p1.PersonId
        };
    }
}