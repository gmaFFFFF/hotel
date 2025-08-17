using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.Error;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.PropertyManagement.Contracts.Repositories;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.business.PersonManagement.BusinessConstraintsChecks;

/// <summary>
///     Удаляемая персона не должна проживать в гостинице
/// </summary>
public class RemovePersonShouldNotSuitable(IHotelBlocksRepositoryFactory<int, Guid> repositoryFactory)
    : IBusinessConstraintCheck<RemovePersonsCommand> {
    public Enum ErrorCode => ErrorBusinessConstraintCheck.CheckPersonLivesInHotel;

    public async Task<bool> IsSatisfiedAsync(RemovePersonsCommand command, CancellationToken cancel = default) {
        var repository = repositoryFactory.CreateTransient();
        var ids = command.Ids.ToList();
        var count = await repository.CountRoomByAsync(
            Room<Guid>.WhereFreeRoom.Not()
                .And(room => room.Visit!.Visitors.Intersect(ids).Any()), cancel);
        return count == 0;
    }
}