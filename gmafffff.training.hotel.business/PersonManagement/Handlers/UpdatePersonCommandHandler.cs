using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.PersonManagement.Contracts.Mappers;
using gmafffff.training.hotel.domain.PersonManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PersonManagement.Dto;
using gmafffff.training.hotel.domain.PersonManagement.Models;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class UpdatePersonCommandHandler(
    IPersonsRepository<Guid> repository,
    IPersonManagementMapper mapper,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<UpdatePersonCommandHandler>? logger = null) :
    UpdateDbCommandHandler<
        UpdatePersonCommand, PersonUpdateDto, UpdatedBusinessEvent<Guid>,
        Person<Guid>, Guid>(repository, mapper, domainEventDispatcher, logger);