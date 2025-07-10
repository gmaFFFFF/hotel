using System;
using System.Linq.Expressions;
using gmafffff.training.hotel.business.PersonManagement.Dto;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.infrastructure.mapper.Required;

namespace gmafffff.training.hotel.infrastructure.mapper.Required
{
    public partial class PersonManagementMapperMapsterBusiness : IPersonManagementMapperMapsterBusiness
    {
        public Expression<Func<Person<Guid>, PersonDto>> EntityToDto => p1 => new PersonDto(p1.Id, p1.FullName.SurName, p1.FullName.FirstName, p1.FullName.Patronymic) {PersonId = p1.Id};
    }
}