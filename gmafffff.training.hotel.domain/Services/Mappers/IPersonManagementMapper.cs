using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.domain.Dto.PersonManagement;

namespace gmafffff.training.hotel.domain.Services.Mappers;

public interface IPersonManagementMapper :
    IEntityMapperForward<Person<Guid>, Guid, PersonDto>,
    IEntityMapperForwardExpression<Person<Guid>, Guid, PersonDto>,
    IEntityMapperBackward<Person<Guid>, Guid, PersonAddDto>,
    IEntityMapperBackward<Person<Guid>, Guid, PersonUpdateDto>;