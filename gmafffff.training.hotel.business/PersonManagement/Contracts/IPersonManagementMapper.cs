using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.business.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.business.PersonManagement.Contracts;

public interface IPersonManagementMapper :
    IEntityMapperForwardExpression<Person<Guid>, Guid, PersonDto>;