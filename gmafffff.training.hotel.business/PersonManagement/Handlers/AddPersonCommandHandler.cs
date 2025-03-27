using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Mappers;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class AddPersonCommandHandler(IPersonsRepository<Guid> repo, IPersonManagementMapper mapper)
    : BusinessCommandDbHandler<AddPersonCommand, CreatedBusinessEvent<Guid>,
        Person<Guid>, Guid, IPersonsRepository<Guid>, Unit, Person<Guid>>(repo) {
    protected override Task<Fin<IList<Person<Guid>>>> RunActionAsync(IList<Unit> loaded,
        CancellationToken cancel = default) {
        var person = mapper.Map(Command.Person)!;
        repo.Add(person);
        return Task.FromResult(Fin<IList<Person<Guid>>>.Succ([person]));
    }

    protected override IList<CreatedBusinessEvent<Guid>> PackResultToEvent(IList<Person<Guid>> result) {
        return result
            .Select(person => new CreatedBusinessEvent<Guid>(person.Id, Command))
            .ToArray();
    }
}