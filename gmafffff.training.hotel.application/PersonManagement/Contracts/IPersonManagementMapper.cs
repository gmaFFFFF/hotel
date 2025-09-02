using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.application.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.application.PersonManagement.Contracts;

public interface IPersonManagementMapper :
    IEntityMapperForwardExpression<Person<Guid>, Guid, PersonDto>;