using gmafffff.starterKit.BusinessLogic.Crud;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class RemovePersonsCommandHandler(IPersonsRepository<Guid> repo)
    : DeleteDbCommandHandler<
        RemovePersonsCommand, DeletedBusinessEvent<Guid>,
        Person<Guid>, Guid>(repo);