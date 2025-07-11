using gmafffff.training.hotel.domain.PersonManagement.Contracts.Mappers;

namespace gmafffff.training.hotel.infrastructure.mapper.Required;

[Mapper]
public interface IPersonManagementMapperMapsterDomain :
    IPersonManagementMapper;

[Mapper]
public interface IPersonManagementMapperMapsterBusiness :
    business.PersonManagement.Contracts.IPersonManagementMapper;