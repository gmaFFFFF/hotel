using gmafffff.starterKit.AppError;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain.Events;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.business.SettlementManagement.Events;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.domain.Model;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.business.SettlementManagement.Handlers;

public class MoveOutCommandHandler(
    IHotelBlocksRepository<int, Guid> repo,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<MoveOutCommandHandler>? logger = null)
    : BusinessCommandDbHandler<MoveOutCommand,
        HotelBlock<int, Guid>, int, IHotelBlocksRepository<int, Guid>,
        HotelBlock<int, Guid>, AccommodationReport<Guid>>(repo, domainEventDispatcher, logger: logger) {
    protected override async Task<Fin<IList<HotelBlock<int, Guid>>>> LoadAsync(IHotelBlocksRepository<int, Guid> repo,
        CancellationToken cancel = default) {
        var found = (await repo
                .LoadWithRoomFilterAsync(predicate: room => room.Id == Command.RoomId, cancel)
                .ConfigureAwait(false))
            .SingleOrDefault();

        return found is null
            ? AppErrorHelper.NewError(AppErrorCode.DbNotFound)
            : Fin<IList<HotelBlock<int, Guid>>>.Succ([found]);
    }

    protected override Task<Fin<IList<AccommodationReport<Guid>>>> RunActionAsync(IList<HotelBlock<int, Guid>> loaded,
        CancellationToken cancel = default) {
        var room = loaded[0].Rooms.Single(room => room.Id == Command.RoomId);
        var report = loaded[0].MoveOut(room, Command.DepartureDate);
        return Task.FromResult(Fin<IList<AccommodationReport<Guid>>>.Succ([report]));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<AccommodationReport<Guid>> result) {
        return result
            .SelectMany(report => new BusinessEvent[] {
                new MovedOutEvent(report, Command),
                new CalculatedPriceForAccommodationTrigger(report, Command)
            })
            .ToList();
    }
}