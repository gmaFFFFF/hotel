using gmafffff.training.hotel.domain.Dto.PersonManagement;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.PersonManagement.Queries;

public record GetPersonsQuery(
    Expression<Func<Person<Guid>, bool>> Filter,
    Func<IQueryable<PersonDto>, IOrderedQueryable<PersonDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : Query<PersonDto>(SortOrder, Pager);