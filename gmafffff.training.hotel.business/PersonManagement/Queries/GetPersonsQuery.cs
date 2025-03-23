using gmafffff.training.hotel.domain.Model;
using Query = gmafffff.starterKit.Messaging.Query;

namespace gmafffff.training.hotel.business.PersonManagement.Queries;

public record GetPersonsQuery(Expression<Func<Person<Guid>, bool>> Filter, (uint pageNum, uint pageSize)? Pager = null)
    : Query(Pager);