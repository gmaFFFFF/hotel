using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.domain.Model;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class RemovePersonsCommandHandler(
    IPersonsRepository<Guid> repo,
    IServiceProvider serviceProvider,
    ILogger<RemovePersonsCommandHandler>? logger = null)
    : DeleteDbCommandHandler<
        RemovePersonsCommand, DeletedBusinessEvent<Guid>,
        Person<Guid>, Guid>(repo, serviceProvider, logger);