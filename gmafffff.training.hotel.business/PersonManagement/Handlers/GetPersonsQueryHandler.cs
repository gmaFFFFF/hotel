using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.training.hotel.business.PersonManagement.Queries;
using gmafffff.training.hotel.domain.Contracts.Mappers;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class GetPersonsQueryHandler(IPersonsRepository<Guid> repo, IPersonManagementMapper mapper) :
    ReadDbQueryHandler<GetPersonsQuery, PersonDto, Person<Guid>, Guid>(repo, mapper);