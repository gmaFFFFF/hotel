using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.Error;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.business.PropertyManagement.BusinessConstraintsChecks;

public class RemoveRoomsMustFree(
    IRepositoryFactory<IHotelBlocksRepository<int, Guid>, HotelBlock<int, Guid>, int> repositoryFactory)
    : IBusinessConstraintCheck<RemoveRoomsCommand> {
    public Enum ErrorCode => ErrorBusinessConstraintCheck.CheckRoomBusy;

    public async Task<bool> IsSatisfiedAsync(RemoveRoomsCommand command, CancellationToken cancel = default) {
        var repository = repositoryFactory.CreateTransient();
        var ids = command.Ids.ToList();
        var count = await repository.CountRoomByAsync(
            Room<Guid>.WhereFreeRoom.Not()
                .And(room => ids.Contains(room.Id)), cancel);
        return count == 0;
    }
}