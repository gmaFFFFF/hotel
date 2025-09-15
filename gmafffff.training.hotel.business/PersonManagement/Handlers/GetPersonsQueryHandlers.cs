using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Mappers;
using gmafffff.training.hotel.business.PersonManagement.Queries;
using gmafffff.training.hotel.domain.PersonManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PersonManagement.Models;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

/// <summary>
///     Универсальный обработчик запросов к БД с фильтрацией по <typeparamref name="TDto" />
/// </summary>
/// <remarks>
///     К сожалению в DI контейнере Microsoft нельзя осуществить сложную регистрацию открытых обобщённых типов.
///     Поэтому для получения типизированного обработчика нужно использовать фабрику <see cref="QueryHandlerFactory" />.
/// </remarks>
public class GetPersonsQueryByDtoHandler<TDto>(
    IPersonsRepository<Guid> repo,
    IEntityMapperForwardExpression<Person<Guid>, Guid, TDto> mapper) :
    ReadDbQueryHandler<GetPersonsQueryByDto<TDto>, Person<Guid>, Guid, IPersonsRepository<Guid>, TDto>(repo, mapper)
    where TDto : class;

/// <summary>
///     Универсальный обработчик запросов к БД с фильтрацией по <see cref="Person{Guid}" />
/// </summary>
/// <remarks>
///     К сожалению в DI контейнере Microsoft нельзя осуществить сложную регистрацию открытых обобщённых типов.
///     Поэтому для получения типизированного обработчика нужно использовать фабрику <see cref="QueryHandlerFactory" />.
/// </remarks>
public class GetPersonsQueryByEntityHandler<TDto>(
    IPersonsRepository<Guid> repo,
    IEntityMapperForwardExpression<Person<Guid>, Guid, TDto> mapper) :
    ReadDbQueryHandler<GetPersonsQueryByEntity<TDto>, Person<Guid>, Guid, IPersonsRepository<Guid>, TDto>(repo, mapper)
    where TDto : class;