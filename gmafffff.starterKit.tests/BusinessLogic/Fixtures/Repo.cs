using System.Collections.Immutable;
using System.Linq.Expressions;
using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.tests.BusinessLogic.Fixtures;

public class Repo : IRepository<BusinessEntity, int> {
    public BusinessEntity? this[int id] => throw new NotImplementedException();

    public IImmutableList<BusinessEntity> this[Expression<Func<BusinessEntity, bool>>? spec] =>
        throw new NotImplementedException();

    public void Add(BusinessEntity entity) {
        throw new NotImplementedException();
    }

    public void Add(IEnumerable<BusinessEntity> entities) {
        throw new NotImplementedException();
    }

    public void Add(object entity) {
        throw new NotImplementedException();
    }

    public void Add(IEnumerable<object> entities) {
        throw new NotImplementedException();
    }

    public int Count() {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public int CountBy(Expression<Func<BusinessEntity, bool>>? spec = null) {
        throw new NotImplementedException();
    }

    public Task<int> CountByAsync(Expression<Func<BusinessEntity, bool>>? spec = null,
        CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public void Delete(BusinessEntity entity) {
        throw new NotImplementedException();
    }

    public void Delete(IEnumerable<BusinessEntity> entities) {
        throw new NotImplementedException();
    }

    public void Delete(int id) {
        throw new NotImplementedException();
    }

    public void Delete(IEnumerable<int> ids) {
        throw new NotImplementedException();
    }

    public void Delete<TChild, TChildId>(TChild entity)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        throw new NotImplementedException();
    }

    public void Delete<TChild, TChildId>(IEnumerable<TChild> entities)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        throw new NotImplementedException();
    }

    public void Delete<TChild, TChildId>(TChildId entityId)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        throw new NotImplementedException();
    }

    public void Delete<TChild, TChildId>(IEnumerable<TChildId> entityIds)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        throw new NotImplementedException();
    }

    public int DeleteBulk(Expression<Func<BusinessEntity, bool>>? spec) {
        throw new NotImplementedException();
    }

    public Task<int> DeleteBulkAsync(Expression<Func<BusinessEntity, bool>>? spec, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public void Dispose() {
        throw new NotImplementedException();
    }

    public IImmutableList<BusinessEntity> Find(IEnumerable<int> ids) {
        throw new NotImplementedException();
    }

    public IImmutableList<BusinessEntity> Find(params int[] ids) {
        throw new NotImplementedException();
    }

    public BusinessEntity? Find(int id) {
        throw new NotImplementedException();
    }

    public Task<IImmutableList<BusinessEntity>> FindAsync(IEnumerable<int> ids, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IImmutableList<BusinessEntity>> FindAsync(CancellationToken cancel = default, params int[] ids) {
        throw new NotImplementedException();
    }

    public Task<BusinessEntity?> FindAsync(int id, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IList<BusinessEntity>> GetAsync(
        Func<IQueryable<BusinessEntity>, IOrderedQueryable<BusinessEntity>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IList<TDto>> GetAsync<TDto>(Expression<Func<BusinessEntity, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class {
        throw new NotImplementedException();
    }

    public Task<IList<BusinessEntity>> GetAsync(Expression<Func<BusinessEntity, bool>> spec,
        Func<IQueryable<BusinessEntity>, IOrderedQueryable<BusinessEntity>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IList<TDto>> GetAsync<TDto>(Expression<Func<BusinessEntity, bool>> spec,
        Expression<Func<BusinessEntity, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class {
        throw new NotImplementedException();
    }

    public Task<IList<BusinessEntity>> GetAsync(IEnumerable<int> ids,
        Func<IQueryable<BusinessEntity>, IOrderedQueryable<BusinessEntity>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IList<TDto>> GetAsync<TDto>(IEnumerable<int> ids, Expression<Func<BusinessEntity, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class {
        throw new NotImplementedException();
    }

    public Task<IList<BusinessEntity>> GetAsync(
        Func<IQueryable<BusinessEntity>, IOrderedQueryable<BusinessEntity>>? sortOrder = null,
        (uint pageNum, uint pageSize)? pager = null, CancellationToken cancel = default, params int[] ids) {
        throw new NotImplementedException();
    }

    public Task<IList<TDto>> GetAsync<TDto>(Expression<Func<BusinessEntity, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default, params int[] ids) where TDto : class {
        throw new NotImplementedException();
    }

    public Task<BusinessEntity?> GetAsync(int id, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<TDto?> GetAsync<TDto>(int id, Expression<Func<BusinessEntity, TDto>> entityToDto,
        CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IList<TDto>> GetAsync<TDto>(Expression<Func<TDto, bool>> spec,
        Expression<Func<BusinessEntity, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class {
        throw new NotImplementedException();
    }

    public IImmutableList<BusinessEntity> Load(Expression<Func<BusinessEntity, bool>>? spec = null) {
        throw new NotImplementedException();
    }

    public IImmutableList<BusinessEntity> Load(IEnumerable<int> ids) {
        throw new NotImplementedException();
    }

    public IImmutableList<BusinessEntity> Load(params int[] ids) {
        throw new NotImplementedException();
    }

    public BusinessEntity? Load(int id) {
        throw new NotImplementedException();
    }

    public Task<IImmutableSet<BusinessEntity>> LoadAsync(CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IImmutableList<BusinessEntity>> LoadAsync(Expression<Func<BusinessEntity, bool>> spec,
        CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IImmutableList<BusinessEntity>> LoadAsync(IEnumerable<int> ids, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IImmutableList<BusinessEntity>> LoadAsync(CancellationToken cancel = default, params int[] ids) {
        throw new NotImplementedException();
    }

    public Task<BusinessEntity?> LoadAsync(int id, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public Task<IImmutableList<BusinessEntity>> LoadOnlyRootAsync(Expression<Func<BusinessEntity, bool>>? spec = null,
        CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public int SaveChanges() {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancel = default) {
        return Task.FromResult(0);
    }

    public void Update(BusinessEntity entity) {
        throw new NotImplementedException();
    }

    public void Update(object entity) {
        throw new NotImplementedException();
    }
}