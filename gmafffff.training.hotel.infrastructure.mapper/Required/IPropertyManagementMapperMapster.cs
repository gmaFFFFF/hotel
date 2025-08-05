using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Mappers;

namespace gmafffff.training.hotel.infrastructure.mapper.Required;

[Mapper]
public interface IPropertyManagementMapperMapsterDomain :
    IPropertyManagementMapper;

[Mapper]
public interface
    IPropertyManagementMapperMapsterBusiness :
    business.PropertyManagement.Contracts.IPropertyManagementMapper;