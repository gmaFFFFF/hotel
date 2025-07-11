using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.business.PersonManagement.Dto;
using gmafffff.training.hotel.business.PersonManagement.Queries;
using gmafffff.training.hotel.domain.PersonManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

/// <summary>
///     Обработчик запросов к БД для типа dto <see cref="PersonDto" />
/// </summary>
/// <remarks>
///     К сожалению в DI контейнере Microsoft нельзя осуществить сложную регистрацию открытых обобщённых типов.
///     Поэтому для каждого DTO нужно создать отдельный обработчик.
/// </remarks>
public class GetPersonsQueryHandler(
    IPersonsRepository<Guid> repo,
    IEntityMapperForwardExpression<Person<Guid>, Guid, PersonDto> mapper) :
    ReadDbQueryHandler<GetPersonsQuery<PersonDto>, Person<Guid>, Guid, PersonDto>(repo, mapper);