using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.domain.Dto.PersonManagement;

namespace gmafffff.training.hotel.business.PersonManagement.Queries;

public record GetPersonsQuery(
    Expression<Func<PersonDto, bool>> Filter,
    Func<IQueryable<PersonDto>, IOrderedQueryable<PersonDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : ReadDbQuery<PersonDto>(Filter, SortOrder, Pager);