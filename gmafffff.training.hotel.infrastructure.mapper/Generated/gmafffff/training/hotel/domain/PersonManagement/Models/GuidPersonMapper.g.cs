using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.application.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.domain.PersonManagement.Models
{
    public static partial class GuidPersonMapper
    {
        public static Expression<Func<Person<Guid>, PersonDto>> ProjectToPersonDto => p1 => new PersonDto(p1.Id, p1.FullName.SurName, p1.FullName.FirstName, p1.FullName.Patronymic) {PersonId = p1.Id};
    }
}