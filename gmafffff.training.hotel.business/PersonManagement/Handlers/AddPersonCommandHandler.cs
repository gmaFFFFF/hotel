using gmafffff.starterKit.BusinessLogic.Crud;
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
    IServiceProvider serviceProvider,
    ILogger<AddPersonCommandHandler>? logger = null)
    : CreateDbCommandHandler<
        AddPersonCommand, PersonAddDto, CreatedBusinessEvent<Guid>,
        Person<Guid>, Guid>(repository, mapper, serviceProvider, logger);