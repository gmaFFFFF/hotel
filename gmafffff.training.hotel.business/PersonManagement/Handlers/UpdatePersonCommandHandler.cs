using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Standard;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Mappers;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class UpdatePersonCommandHandler(IPersonsRepository<Guid> repo, IPersonManagementMapper mapper)
    : BusinessCommandDbHandler<UpdatePersonCommand, UpdatedBusinessEvent<Guid>, Person<Guid>> {
    protected override async Task<Fin<Unit>> LoadAsync(CancellationToken cancel = default) {
        var found = await repo
            .LoadAsync(LastCommand!.Id, cancel)
            .ConfigureAwait(false);

        if (found is null)
            return AppErrorHelper.NewError(AppErrorCode.DbNotFound);

        PreliminaryResult = [found];

        return Unit.Default;
    }

    protected override Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default) {
        mapper.Update(LastCommand!.PersonUpdate, PreliminaryResult[0]);
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }

    protected override async Task<Fin<Unit>> SaveAsync(CancellationToken cancel = default) {
        await repo.SaveChangesAsync(cancel);
        return Unit.Default;
    }

    protected override void PackResultToEvent() {
        LastResult = PreliminaryResult
            .Select(person => new UpdatedBusinessEvent<Guid>(person.Id, LastCommand!))
            .ToList();
    }
}