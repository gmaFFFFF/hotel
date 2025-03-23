using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.Error;
using gmafffff.training.hotel.business.PersonManagement.Commands;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.BusinessRules;

/// <summary>
///     Удаляемая персона не должна проживать в гостинице
/// </summary>
public class RemovePersonShouldNotSuitable(IHotelBlocksRepositoryFactory<int, Guid> repositoryFactory)
    : IBusinessRule<RemovePersonsCommand> {
    public Enum ErrorCode => ViolationBusinessRules.PersonLivesInHotel;

    public async Task<bool> IsSatisfiedAsync(RemovePersonsCommand command, CancellationToken cancel = default) {
        var repository = repositoryFactory.CreateTransient();
        var ids = command.Ids.ToList();
        var count = await repository.CountRoomByAsync(
            Room<Guid>.IsFreeRoom.Not()
                .And(room => room.Visit!.Visitors.Intersect(ids).Any()));
        return count == 0;
    }
}