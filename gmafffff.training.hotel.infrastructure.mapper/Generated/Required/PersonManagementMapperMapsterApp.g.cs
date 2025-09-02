using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.application.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;
using gmafffff.training.hotel.infrastructure.mapper.Required;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PersonManagementMapperMapsterApp : IPersonManagementMapperMapsterApp
    {
        public Expression<Func<Person<Guid>, PersonDto>> EntityToDto => p1 => new PersonDto(p1.Id, p1.FullName.SurName, p1.FullName.FirstName, p1.FullName.Patronymic) {PersonId = p1.Id};
    }
}