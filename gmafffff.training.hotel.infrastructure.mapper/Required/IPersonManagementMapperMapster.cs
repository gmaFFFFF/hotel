namespace gmafffff.training.hotel.infrastructure.mapper.Required;

[Mapper]
public interface IPersonManagementMapperMapsterDomain :
    domain.Contracts.Mappers.IPersonManagementMapper;

[Mapper]
public interface IPersonManagementMapperMapsterBusiness :
    business.PersonManagement.Contracts.IPersonManagementMapper;