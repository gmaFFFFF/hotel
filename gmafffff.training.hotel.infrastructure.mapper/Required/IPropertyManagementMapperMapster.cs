namespace gmafffff.training.hotel.infrastructure.mapper.Required;

[Mapper]
public interface IPropertyManagementMapperMapsterDomain :
    domain.PropertyManagement.Contracts.Mappers.IPropertyManagementMapper;

[Mapper]
public interface IPropertyManagementMapperMapsterAppRoom :
    application.PropertyManagement.Contracts.IPropertyManagementMapperRoom;

[Mapper]
public interface IPropertyManagementMapperMapsterAppHotelBlock :
    application.PropertyManagement.Contracts.IPropertyManagementMapperHotelBlock;