using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.business.SettlementManagement.Events;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.domain.Model;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.business.SettlementManagement.Handlers;

public class SettleInCommandHandler(
    IHotelBlocksRepository<int, Guid> repo,
    IPersonsRepository<Guid> personsRepository,
    IServiceProvider serviceProvider,
    ILogger<SettleInCommandHandler>? logger = null)
    : BusinessCommandDbHandler<SettleInCommand,
        HotelBlock<int, Guid>, int, IHotelBlocksRepository<int, Guid>,
        HotelBlock<int, Guid>, Room<Guid>>(repo, serviceProvider, logger: logger) {
    protected override async Task<Fin<IList<HotelBlock<int, Guid>>>> LoadAsync(IHotelBlocksRepository<int, Guid> repo,
        CancellationToken cancel = default) {
        var found = (await repo
                .LoadWithRoomFilterAsync(predicate: room => room.Id == Command.RoomId, cancel)
                .ConfigureAwait(false))
            .SingleOrDefault();
        var persons =
            await personsRepository.GetAsync(Command.Visitors, entityToDto: p => new { p.Id }, cancel: cancel);
        var notFoundPersons = Command.Visitors.Except(persons.Select(p => p.Id));

        return found is null || notFoundPersons.Any()
            ? AppErrorHelper.NewError(AppErrorCode.DbNotFound)
            : Fin<IList<HotelBlock<int, Guid>>>.Succ([found]);
    }

    protected override Task<Fin<IList<Room<Guid>>>> RunActionAsync(IList<HotelBlock<int, Guid>> loaded,
        CancellationToken cancel = default) {
        var room = loaded[0].Rooms.Single();
        _ = loaded[0].SettledIn(Command.Visitors, room, departureDatePlanned: Command.DepartureDatePlanned);
        return Task.FromResult(Fin<IList<Room<Guid>>>.Succ([room]));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<Room<Guid>> result) {
        return result
            .Select(room => new SettledInEvent(room.Id, Command))
            .Cast<BusinessEvent>()
            .ToList();
    }
}