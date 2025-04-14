using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.Error;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Contracts.Repositories;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.PropertyManagement.BusinessRules;

public class RemoveRoomsMustFree(IHotelBlocksRepositoryFactory<int, Guid> repositoryFactory)
    : IBusinessRule<RemoveRoomsCommand> {
    public Enum ErrorCode => ErrorBusinessRules.RuleRoomBusy;

    public async Task<bool> IsSatisfiedAsync(RemoveRoomsCommand command, CancellationToken cancel = default) {
        var repository = repositoryFactory.CreateTransient();
        var ids = command.Ids.ToList();
        var count = await repository.CountRoomByAsync(
            Room<Guid>.IsFreeRoom.Not()
                .And(room => ids.Contains(room.Id)), cancel);
        return count == 0;
    }
}