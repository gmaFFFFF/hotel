using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Standard;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Mappers;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PropertyManagement.Handlers;

public class AddRoomCommandHandler(IHotelBlocksRepository<int, Guid> repo, IPropertyManagementMapper mapper)
    : BusinessCommandDbHandler<AddRoomCommand, CreatedBusinessEvent<int>, Room<Guid>> {
    private HotelBlock<int, Guid>? _block;

    protected override async Task<Fin<Unit>> LoadAsync(CancellationToken cancel = default) {
        var loaded = await repo
            .LoadOnlyRootAsync(spec: hotel => hotel.Id == LastCommand!.Room.HotelBlockId, cancel);

        if (loaded.Count == 0)
            return AppErrorHelper.NewError(AppErrorCode.DbNotFound);

        _block = loaded[0];
        return Unit.Default;
    }

    protected override Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default) {
        var newRoom = mapper.Map(LastCommand!.Room);
        _block!.Rooms = [newRoom];
        PreliminaryResult = [newRoom];
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }

    protected override async Task<Fin<Unit>> SaveAsync(CancellationToken cancel = default) {
        await repo.SaveChangesAsync(cancel);
        return Unit.Default;
    }

    protected override void PackResultToEvent() {
        LastResult = PreliminaryResult
            .Select(room => new CreatedBusinessEvent<int>(room.Id, LastCommand!))
            .ToArray();
    }
}