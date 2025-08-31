using gmafffff.training.hotel.business.PropertyManagement.Contracts;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Mappers;

namespace gmafffff.training.hotel.infrastructure.mapper.Required;

[Mapper]
public interface IPropertyManagementMapperMapsterDomain :
    IPropertyManagementMapper;

[Mapper]
public interface IPropertyManagementMapperMapsterBusinessRoom :
    IPropertyManagementMapperRoom;

[Mapper]
public interface IPropertyManagementMapperMapsterBusinessHotelBlock :
    IPropertyManagementMapperHotelBlock;