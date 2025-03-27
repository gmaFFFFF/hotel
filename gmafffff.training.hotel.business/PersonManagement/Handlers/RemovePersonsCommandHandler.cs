using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class RemovePersonsCommandHandler(IPersonsRepository<Guid> repo)
    : BusinessCommandDbHandler<RemovePersonsCommand, RemovedBusinessEvent<Guid>,
        Person<Guid>, Guid, IPersonsRepository<Guid>, Person<Guid>, Person<Guid>>(repo) {
    protected override async Task<Fin<IList<Person<Guid>>>> LoadAsync(IPersonsRepository<Guid> repo,
        CancellationToken cancel = default) {
        return (await repo
                .LoadAsync(spec: person => Command.Ids.Contains(person.Id), cancel)
                .ConfigureAwait(false))
            .ToList();
    }

    protected override Task<Fin<IList<Person<Guid>>>> RunActionAsync(IList<Person<Guid>> loaded,
        CancellationToken cancel = default) {
        repo.Delete(loaded);
        return Task.FromResult(Fin<IList<Person<Guid>>>.Succ(loaded));
    }

    protected override IList<RemovedBusinessEvent<Guid>> PackResultToEvent(IList<Person<Guid>> result) {
        return result
            .Select(person => new RemovedBusinessEvent<Guid>(person.Id, Command))
            .ToList();
    }
}