using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.PersonManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PersonManagement.Models;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class RemovePersonsCommandHandler(
    IPersonsRepository<Guid> repo,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<RemovePersonsCommandHandler>? logger = null)
    : DeleteDbCommandHandler<
        RemovePersonsCommand, DeletedBusinessEvent<Guid>,
        Person<Guid>, Guid>(repo, domainEventDispatcher, logger);