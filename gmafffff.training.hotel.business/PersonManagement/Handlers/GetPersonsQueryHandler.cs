using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.training.hotel.business.PersonManagement.Queries;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Mappers;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class GetPersonsQueryHandler(IPersonsRepository<Guid> repo, IPersonManagementMapper mapper) :
    ReadDbQueryHandler<GetPersonsQuery, PersonDto, Person<Guid>, Guid>(repo, mapper);