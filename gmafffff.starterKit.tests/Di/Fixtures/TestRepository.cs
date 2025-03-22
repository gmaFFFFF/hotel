using System.Collections.Immutable;
using System.Linq.Expressions;
using gmafffff.starterKit.Domain;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestRepository : ITestRepository {
    public void Dispose() {
        throw new NotImplementedException();
    }

    public async Task<IImmutableList<TestEntity>> LoadAllAsync(CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public async Task<IImmutableList<TestEntity>> GetAllDetachAsync(CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public IImmutableList<TestEntity> Find(IEnumerable<int> ids) {
        throw new NotImplementedException();
    }

    public IImmutableList<TestEntity> Find(params int[] ids) {
        throw new NotImplementedException();
    }

    public TestEntity? Find(int id) {
        throw new NotImplementedException();
    }

    public async Task<IImmutableList<TestEntity>> FindAsync(IEnumerable<int> ids, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public async Task<IImmutableList<TestEntity>> FindAsync(CancellationToken cancel = default, params int[] ids) {
        throw new NotImplementedException();
    }

    public async Task<TestEntity?> FindAsync(int id, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public IImmutableList<TestEntity> Load(Expression<Func<TestEntity, bool>>? spec = null) {
        throw new NotImplementedException();
    }

    public IImmutableList<TestEntity> Load(IEnumerable<int> ids) {
        throw new NotImplementedException();
    }

    public IImmutableList<TestEntity> Load(params int[] ids) {
        throw new NotImplementedException();
    }

    public TestEntity? Load(int id) {
        throw new NotImplementedException();
    }

    public async Task<IImmutableList<TestEntity>> LoadAsync(Expression<Func<TestEntity, bool>>? spec = null,
        CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public async Task<IImmutableList<TestEntity>> LoadAsync(IEnumerable<int> ids, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public async Task<IImmutableList<TestEntity>> LoadAsync(CancellationToken cancel = default, params int[] ids) {
        throw new NotImplementedException();
    }

    public async Task<TestEntity?> LoadAsync(int id, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public TestEntity? this[int id] {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IImmutableList<TestEntity> this[Expression<Func<TestEntity, bool>>? spec] {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public async Task<IImmutableList<TestEntity>> LoadOnlyRootAsync(Expression<Func<TestEntity, bool>>? spec = null,
        CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public void Add(TestEntity entity) {
        throw new NotImplementedException();
    }

    public void Add(IEnumerable<TestEntity> entities) {
        throw new NotImplementedException();
    }

    public void Add(object entity) {
        throw new NotImplementedException();
    }

    public void Add(IEnumerable<object> entities) {
        throw new NotImplementedException();
    }

    public void Delete(TestEntity entity) {
        throw new NotImplementedException();
    }

    public void Delete(IEnumerable<TestEntity> entities) {
        throw new NotImplementedException();
    }

    public void Delete(int id) {
        throw new NotImplementedException();
    }

    public void Delete(IEnumerable<int> ids) {
        throw new NotImplementedException();
    }

    public void Delete<TChild, TChildId>(TChild entity) where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        throw new NotImplementedException();
    }

    public void Delete<TChild, TChildId>(IEnumerable<TChild> entities) where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        throw new NotImplementedException();
    }

    public void Delete<TChild, TChildId>(TChildId entityId) where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        throw new NotImplementedException();
    }

    public void Delete<TChild, TChildId>(IEnumerable<TChildId> entityIds) where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        throw new NotImplementedException();
    }

    public int DeleteBulk(Expression<Func<TestEntity, bool>>? spec) {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteBulkAsync(Expression<Func<TestEntity, bool>>? spec,
        CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public void Update(TestEntity entity) {
        throw new NotImplementedException();
    }

    public void Update(object entity) {
        throw new NotImplementedException();
    }

    public int Count() {
        throw new NotImplementedException();
    }

    public async Task<int> CountAsync(CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public int CountBy(Expression<Func<TestEntity, bool>>? spec = null) {
        throw new NotImplementedException();
    }

    public async Task<int> CountByAsync(Expression<Func<TestEntity, bool>>? spec = null,
        CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    public int SaveChanges() {
        throw new NotImplementedException();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancel = default) {
        throw new NotImplementedException();
    }
}