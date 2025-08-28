using System.Collections.Immutable;
using System.Linq.Expressions;
using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace gmafffff.starterKit.EntityFrameworkCore;

/// <summary>
///     Реализация <see cref="IRepository{T,TId}" /> для EF Core
/// </summary>
public class Repository<T, TId> : RepositoryReadOnly<T, TId>, IRepository<T, TId>
    where T : Entity<TId>
    where TId : struct, IEquatable<TId> {
    #region == Запросы

    #region Локальный поиск с последующим запросом центрального склада

    #region Синхронно

    public IImmutableList<T> Find(IEnumerable<TId> ids) {
        var locals = Entities.Local.Where(e => ids.Contains(e.Id)).ToList();
        var remainderIds = ids.Except(locals.Select(e => e.Id));
        return locals.Concat(Load(remainderIds)).ToImmutableList();
    }

    public IImmutableList<T> Find(params TId[] ids) {
        return Find(ids.AsEnumerable());
    }

    public T? Find(TId id) {
        return Entities.Find(id);
    }

    #endregion

    #region Асинхронно

    public async Task<IImmutableList<T>> FindAsync(IEnumerable<TId> ids, CancellationToken cancel = default) {
        var locals = Entities.Local.Where(e => ids.Contains(e.Id)).ToList();
        var remainderIds = ids.Except(locals.Select(e => e.Id));
        var remote = await LoadAsync(remainderIds, cancel).ConfigureAwait(false);
        return locals.Concat(remote).ToImmutableList();
    }

    public async Task<T?> FindAsync(TId id, CancellationToken cancel = default) {
        return await Entities.FindAsync([id], cancel).ConfigureAwait(false);
    }

    public async Task<IImmutableList<T>> FindAsync(CancellationToken cancel = default, params TId[] ids) {
        return await FindAsync(ids.AsEnumerable(), cancel).ConfigureAwait(false);
    }

    #endregion

    #endregion


    #region Загрузка в оперативный склад

    #region Синхронно

    public IImmutableList<T> Load(Expression<Func<T, bool>>? spec = null) {
        return spec is null
            ? ImmutableArray<T>.Empty
            : QueryBuilder(LoadAll, spec).ToImmutableList();
    }

    public IImmutableList<T> Load(IEnumerable<TId> ids) {
        return Load(e => ids.Contains(e.Id));
    }

    public IImmutableList<T> Load(params TId[] ids) {
        return Load(ids.AsEnumerable());
    }

    public T? Load(TId id) {
        Expression<Func<T, bool>> spec = e => e.Id.Equals(id);
        return LoadAll.SingleOrDefault(spec);
    }

    #endregion

    #region Асинхронно

    public async Task<IImmutableSet<T>> LoadAsync(CancellationToken cancel = default) {
        return [.. await RunBigQueryAsync(LoadAll, cancel).ConfigureAwait(false)];
    }

    public async Task<IImmutableList<T>> LoadAsync(Expression<Func<T, bool>> spec,
        CancellationToken cancel = default) {
        return spec is null
            ? ImmutableArray<T>.Empty
            : [.. await RunQueryAsync(QueryBuilder(LoadAll, spec), cancel).ConfigureAwait(false)];
    }

    public async Task<IImmutableList<T>> LoadAsync(IEnumerable<TId> ids,
        CancellationToken cancel = default) {
        return await LoadAsync(spec: e => ids.Contains(e.Id), cancel).ConfigureAwait(false);
    }

    public async Task<IImmutableList<T>> LoadAsync(CancellationToken cancel = default,
        params TId[] ids) {
        return await LoadAsync(ids.AsEnumerable(), cancel).ConfigureAwait(false);
    }

    public async Task<T?> LoadAsync(TId id,
        CancellationToken cancel = default) {
        Expression<Func<T, bool>> spec = e => e.Id.Equals(id);
        return await LoadAll.SingleOrDefaultAsync(spec, cancel).ConfigureAwait(false);
    }

    #endregion

    #endregion


    #region Загрузка в оперативный склад без связанных сущностей

    public async Task<IImmutableList<T>> LoadOnlyRootAsync(Expression<Func<T, bool>>? spec = null,
        CancellationToken cancel = default) {
        return spec is null
            ? ImmutableArray<T>.Empty
            : [.. await RunQueryAsync(QueryBuilder(Entities, spec), cancel).ConfigureAwait(false)];
    }

    #endregion

    #region Индексатор

    public T? this[TId id] => Find(id);

    public IImmutableList<T> this[Expression<Func<T, bool>>? spec] => Load(spec);

    #endregion

    #endregion


    #region == Управление

    #region Добавление

    public void Add(T entity) {
        Entities.Add(entity);
    }

    public void Add(IEnumerable<T> entities) {
        Entities.AddRange(entities);
    }

    public void Add(object entity) {
        Context.Add(entity);
    }

    public void Add(IEnumerable<object> entities) {
        Context.AddRange(entities);
    }

    #endregion

    #region Удаление

    public void Delete(T entity) {
        Entities.Remove(entity);
    }

    public void Delete(IEnumerable<T> entities) {
        Entities.RemoveRange(entities);
    }

    public void Delete(TId id) {
        if (CreateDeletableEntity<T, TId>(id) is { } entity)
            Delete(entity);
    }

    public void Delete(IEnumerable<TId> ids) {
        foreach (var id in ids)
            Delete(id);
    }

    public void Delete<TChild, TChildId>(TChild entity)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        Context.Remove(entity);
    }

    public void Delete<TChild, TChildId>(IEnumerable<TChild> entities)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        Context.RemoveRange(entities);
    }

    public void Delete<TChild, TChildId>(TChildId id)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        if (CreateDeletableEntity<TChild, TChildId>(id) is { } entity)
            Delete<TChild, TChildId>(entity);
    }

    public void Delete<TChild, TChildId>(IEnumerable<TChildId> ids)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId> {
        foreach (var id in ids)
            Delete<TChild, TChildId>(id);
    }

    public int DeleteBulk(Expression<Func<T, bool>>? spec) {
        return spec is null
            ? 0
            : Entities.Where(spec).ExecuteDelete();
    }


    public async Task<int> DeleteBulkAsync(Expression<Func<T, bool>>? spec,
        CancellationToken cancel = default) {
        return spec is null
            ? 0
            : await Entities.Where(spec).ExecuteDeleteAsync(cancel).ConfigureAwait(false);
    }

    private TEntity? CreateDeletableEntity<TEntity, TEntityId>(TEntityId id)
        where TEntity : Entity<TEntityId>
        where TEntityId : struct, IEquatable<TEntityId> {
        if (Context.Set<TEntity>().Local.FirstOrDefault(e => e.Id.Equals(id)) is { } entity)
            return entity;

        if (Create(id) is { } deletable)
            return deletable;

        // TODO: Синхронный поход в базу — плохо. Подумать над вариантами
        // * async
        // * ограничение на конструктор «new»
        // * exception
        if (Context.Set<TEntity>().SingleOrDefault(e => e.Id.Equals(id)) is { } entityDb)
            return entityDb;

        TEntity? Create(TEntityId id) {
            try {
                var deletableEntity = Activator.CreateInstance<TEntity>();
                deletableEntity.Id = id;
                Context.Attach(deletableEntity);
                return deletableEntity;
            }
            catch {
                // ignored
            }

            return null;
        }

        return null;
    }

    #endregion

    #region Изменение сущностей

    public void Update(T entity) {
        Entities.Update(entity);
    }

    public void Update(object entity) {
        Context.Update(entity);
    }

    #endregion

    #region Сохранение

    public int SaveChanges() {
        return Context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancel = default) {
        return await Context.SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    #endregion

    #endregion


    #region Инициализация

    /// <summary>
    ///     Приемник событий домена
    /// </summary>
    public IDomainEventSink? DomainEventSink { get; }

    /// <summary>
    ///     Запрос на загрузку данных в оперативный склад
    /// </summary>
    protected readonly IQueryable<T> LoadAll;

    public Repository(DbContext dbContext)
        : this(dbContext, domainEventSink: null, autoInclude: null) { }

    public Repository(DbContext dbContext, Func<IQueryable<T>, IIncludableQueryable<T, object>> autoInclude)
        : this(dbContext, domainEventSink: null, autoInclude) { }

    public Repository(DbContext dbContext,
        IDomainEventSink? domainEventSink,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? autoInclude = null)
        : base(dbContext, autoInclude) {
        DomainEventSink = domainEventSink;
        DomainEventSink?.RegisterDbContext(Context);

        LoadAll = QueryBuilder(Entities, include: AutoInclude);
    }

    #endregion
}