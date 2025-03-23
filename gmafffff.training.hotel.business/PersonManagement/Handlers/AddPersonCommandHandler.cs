using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Standard;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Mappers;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class AddPersonCommandHandler(IPersonsRepository<Guid> repo, IPersonManagementMapper mapper)
    : BusinessCommandDbHandler<AddPersonCommand, CreatedBusinessEvent<Guid>, Person<Guid>> {
    protected override Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default) {
        var person = mapper.Map(LastCommand!.Person)!;
        repo.Add(person);
        PreliminaryResult = [person];
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }

    protected override async Task<Fin<Unit>> SaveAsync(CancellationToken cancel = default) {
        await repo.SaveChangesAsync(cancel);
        return Unit.Default;
    }

    protected override void PackResultToEvent() {
        LastResult = PreliminaryResult
            .Select(person => new CreatedBusinessEvent<Guid>(person.Id, LastCommand!))
            .ToArray();
    }
}