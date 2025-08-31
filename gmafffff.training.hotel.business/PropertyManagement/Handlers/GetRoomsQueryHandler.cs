using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.business.PropertyManagement.Queries;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class GetRoomsQueryHandler<TDto>(
    IRoomRepositoryReadOnly repo,
    IEntityMapperForwardExpression<Room<Guid>, int, TDto> mapper)
    : ReadDbQueryHandler<GetRoomsQuery<TDto>, Room<Guid>, int, TDto>(repo, mapper)
    where TDto : class;