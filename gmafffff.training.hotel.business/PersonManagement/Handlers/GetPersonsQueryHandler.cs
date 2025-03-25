using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.PersonManagement.Queries;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Services.Repositories;

namespace gmafffff.training.hotel.business.PersonManagement.Handlers;

public class GetPersonsQueryHandler(IPersonsRepository<Guid> repo) :
    QueryDbHandler<GetPersonsQuery, PersonDto> {
    public override async Task<IList<PersonDto>> CreateDbQuery(GetPersonsQuery query,
        CancellationToken cancel = default) {
        return await repo
            .GetPersonsAsync(query.Filter, query.Pager, cancel)
            .ConfigureAwait(false);
    }
}