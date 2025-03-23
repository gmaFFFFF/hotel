using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Standard;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Mappers;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class UpdateRoomCommandHandler(IHotelBlocksRepository<int, Guid> repo, IPropertyManagementMapper mapper)
    : BusinessCommandDbHandler<UpdateRoomCommand, UpdatedBusinessEvent<int>, Room<Guid>> {
    protected override async Task<Fin<Unit>> LoadAsync(CancellationToken cancel = default) {
        var found = (await repo
                .LoadWithRoomFilterAsync(predicate: room => room.Id == LastCommand!.Id, cancel)
                .ConfigureAwait(false))
            .SingleOrDefault()
            ?.Rooms
            .SingleOrDefault();

        if (found is null)
            return AppErrorHelper.NewError(AppErrorCode.DbNotFound);

        PreliminaryResult = [found];

        return Unit.Default;
    }

    protected override Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default) {
        mapper.Update(LastCommand!.RoomUpdate, PreliminaryResult[0]);
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }

    protected override async Task<Fin<Unit>> SaveAsync(CancellationToken cancel = default) {
        await repo.SaveChangesAsync(cancel);
        return Unit.Default;
    }

    protected override void PackResultToEvent() {
        LastResult = PreliminaryResult
            .Select(room => new UpdatedBusinessEvent<int>(room.Id, LastCommand!))
            .ToList();
    }
}