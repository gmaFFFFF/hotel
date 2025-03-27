using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Mappers;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class UpdatePersonCommandHandler(IPersonsRepository<Guid> repo, IPersonManagementMapper mapper)
    : BusinessCommandDbHandler<UpdatePersonCommand, UpdatedBusinessEvent<Guid>,
        Person<Guid>, Guid, IPersonsRepository<Guid>, Person<Guid>, Person<Guid>>(repo) {
    protected override async Task<Fin<IList<Person<Guid>>>> LoadAsync(IPersonsRepository<Guid> repo,
        CancellationToken cancel = default) {
        var found = await repo
            .LoadAsync(Command.Id, cancel)
            .ConfigureAwait(false);

        return found is null
            ? AppErrorHelper.NewError(AppErrorCode.DbNotFound)
            : Fin<IList<Person<Guid>>>.Succ([found]);
    }

    protected override Task<Fin<IList<Person<Guid>>>> RunActionAsync(IList<Person<Guid>> loaded,
        CancellationToken cancel = default) {
        mapper.Update(Command.PersonUpdate, loaded[0]);
        return Task.FromResult(Fin<IList<Person<Guid>>>.Succ(loaded));
    }

    protected override IList<UpdatedBusinessEvent<Guid>> PackResultToEvent(IList<Person<Guid>> result) {
        return result
            .Select(person => new UpdatedBusinessEvent<Guid>(person.Id, Command))
            .ToList();
    }
}