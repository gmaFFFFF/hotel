using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.business.PersonManagement.Dto;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.PersonManagement.Contracts;

public interface IPersonManagementMapper :
    IEntityMapperForwardExpression<Person<Guid>, Guid, PersonDto>;