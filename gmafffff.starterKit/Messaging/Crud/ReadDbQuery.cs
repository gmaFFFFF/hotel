using System.Linq.Expressions;

namespace gmafffff.starterKit.Messaging.Crud;

public abstract record ReadDbQuery<TDto>(
    Expression<Func<TDto, bool>> Filter,
    Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? SortOrder = null,
    (uint pageNum, uint pageSize)? Pager = null,
    Guid MessageId = default) : Query<TDto>(SortOrder, Pager, MessageId);