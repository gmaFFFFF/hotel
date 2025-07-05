using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.Error;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Contracts.Repositories;

namespace gmafffff.training.hotel.business.PropertyManagement.BusinessConstraintsChecks;

public class RoomNumberMustUnique(IHotelBlocksRepositoryFactory<int, Guid> repositoryFactory)
    : IBusinessConstraintCheck<UpdateRoomCommand>,
        IBusinessConstraintCheck<AddRoomCommand> {
    public async Task<bool> IsSatisfiedAsync(AddRoomCommand command, CancellationToken cancel = default) {
        var repository = repositoryFactory.CreateTransient();
        var count = await repository.CountRoomByAsync(predicate: room => room.RoomDetails.Number == command.Room.Number,
            cancel);
        return count == 0;
    }

    public Enum ErrorCode => ErrorBusinessConstraintCheck.CheckRoomNumberRepeat;

    public async Task<bool> IsSatisfiedAsync(UpdateRoomCommand command, CancellationToken cancel = default) {
        var updateRoomNum = command.RoomUpdate.Number;

        var repository = repositoryFactory.CreateTransient();
        var old = (await repository.GetRoomsAsync(predicate: room => room.Id == command.Id, cancel: cancel))
            .SingleOrDefault();
        if (old is null || old.RoomDetails.Number == updateRoomNum)
            return true;
        var count = await repository.CountRoomByAsync(predicate: room => room.RoomDetails.Number == updateRoomNum,
            cancel);
        return count == 0;
    }
}