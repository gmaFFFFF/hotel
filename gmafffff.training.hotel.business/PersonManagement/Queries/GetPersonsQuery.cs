using gmafffff.starterKit.Messaging.Crud;

namespace gmafffff.training.hotel.business.PersonManagement.Queries;

public record GetPersonsQuery<TPersonDto>(
    Expression<Func<TPersonDto, bool>> Filter,
    Func<IQueryable<TPersonDto>, IOrderedQueryable<TPersonDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null)
    : ReadDbQuery<TPersonDto>(Filter, SortOrder, Pager);