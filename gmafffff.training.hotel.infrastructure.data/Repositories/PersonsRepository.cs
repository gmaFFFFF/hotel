using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.infrastructure.data.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.Repositories;

public class PersonsRepository(HotelDbContext context, IPersonManagementMapper managementMapper) :
    Repository<Person<Guid>, Guid>(context),
    IPersonsRepository<Guid> {
    public async Task<IList<PersonDto>> GetPersonsAsync(Expression<Func<Person<Guid>, bool>> predicate,
        (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) {
        var query = QueryBuilder(GetAll.Where(predicate), pager: pager);
        return await RunQueryAsync(query.Select(managementMapper.EntityToDto), cancel)
            .ConfigureAwait(false);
    }
}