using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Standard;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class RemovePersonsCommandHandler(IPersonsRepository<Guid> repo)
    : BusinessCommandDbHandler<RemovePersonsCommand, RemovedBusinessEvent<Guid>, Person<Guid>> {
    protected override async Task<Fin<Unit>> LoadAsync(CancellationToken cancel = default) {
        PreliminaryResult = (await repo.LoadAsync(spec: person => LastCommand!.Ids.Contains(person.Id), cancel))
            .ToList();
        return Unit.Default;
    }

    protected override Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default) {
        repo.Delete(PreliminaryResult);
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }

    protected override async Task<Fin<Unit>> SaveAsync(CancellationToken cancel = default) {
        await repo.SaveChangesAsync(cancel);
        return Unit.Default;
    }

    protected override void PackResultToEvent() {
        LastResult = PreliminaryResult
            .Select(person => new RemovedBusinessEvent<Guid>(person.Id, LastCommand!))
            .ToList();
    }
}