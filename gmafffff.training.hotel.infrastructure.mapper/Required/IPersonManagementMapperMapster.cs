namespace gmafffff.training.hotel.infrastructure.mapper.Required;

[Mapper]
public interface IPersonManagementMapperMapsterDomain :
    domain.PersonManagement.Contracts.Mappers.IPersonManagementMapper;

[Mapper]
public interface IPersonManagementMapperMapsterApp :
    application.PersonManagement.Contracts.IPersonManagementMapper;