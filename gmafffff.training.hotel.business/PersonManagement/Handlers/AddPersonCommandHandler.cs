using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Contracts.Mappers;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class AddPersonCommandHandler(
    IPersonsRepository<Guid> repository,
    IPersonManagementMapper mapper,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<AddPersonCommandHandler>? logger = null)
    : CreateDbCommandHandler<
        AddPersonCommand, PersonAddDto, CreatedBusinessEvent<Guid>,
        Person<Guid>, Guid>(repository, mapper, domainEventDispatcher, logger);