using System;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.domain.Dto.PersonManagement
{
    public static partial class PersonAddDtoMapper
    {
        public static Person<Guid> AdaptToGuidPerson(this PersonAddDto p1)
        {
            return p1 == null ? null : new Person<Guid>() {FullName = p1 == null ? null : new PersonFullName(p1.SurName, p1.FirstName, p1.Patronymic) {}};
        }
    }
}