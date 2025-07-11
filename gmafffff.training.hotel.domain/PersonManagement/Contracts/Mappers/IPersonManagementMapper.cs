using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.PersonManagement.Dto;

namespace gmafffff.training.hotel.domain.PersonManagement.Contracts.Mappers;

public interface IPersonManagementMapper :
    IEntityMapperBackward<Person<Guid>, Guid, PersonAddDto>,
    IEntityMapperBackward<Person<Guid>, Guid, PersonUpdateDto>;