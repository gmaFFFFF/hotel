using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.Dto.PersonManagement;

namespace gmafffff.training.hotel.domain.Contracts.Mappers;

public interface IPersonManagementMapper :
    IEntityMapperBackward<Person<Guid>, Guid, PersonAddDto>,
    IEntityMapperBackward<Person<Guid>, Guid, PersonUpdateDto>;