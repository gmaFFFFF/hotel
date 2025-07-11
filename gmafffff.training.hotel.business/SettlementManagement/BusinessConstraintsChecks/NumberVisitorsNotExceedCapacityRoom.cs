using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.Error;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;

namespace gmafffff.training.hotel.business.SettlementManagement.BusinessConstraintsChecks;

/// <summary>
///     Число посетителей не превышает вместимость номера
/// </summary>
public class NumberVisitorsNotExceedCapacityRoom(IHotelBlocksRepositoryFactory<int, Guid> repositoryFactory)
    : IBusinessConstraintCheck<SettleInCommand> {
    public Enum ErrorCode => ErrorBusinessConstraintCheck.CheckPersonLivesInHotel;

    public async Task<bool> IsSatisfiedAsync(SettleInCommand command, CancellationToken cancel = default) {
        var repository = repositoryFactory.CreateTransient();
        var room = (await repository.LoadWithRoomFilterAsync(predicate: r => r.Id == command.RoomId, cancel))
            .Single()
            .Rooms
            .Single(r => r.Id == command.RoomId);
        var currentVisitorsCount = room.Visit?.Visitors.Count ?? 0;
        var plannedVisitorsCount = currentVisitorsCount + command.Visitors.Count();
        return plannedVisitorsCount <= room.RoomDetails.Capacity;
    }
}