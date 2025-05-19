using System.Collections.Immutable;
using System.Linq.Expressions;
using gmafffff.starterKit.Domain;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace gmafffff.starterKit.EntityFrameworkCore;

/// <summary>
///     Реализация <see cref="IRepository{T,TId}" /> для EF Core
/// </summary>
public class Repository<T, TId> : IRepository<T, TId>
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
        return await Entities.FindAsync(id).ConfigureAwait(false);
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

    #region Выгрузка данных потребителю без постановки на учет в оперативном складе

    public async Task<IList<T>> GetAsync(
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) {
        var query = QueryBuilder(GetAll, sortOrder: sortOrder, pager: pager);

        return await RunQueryAsync(query, cancel).ConfigureAwait(false);
    }

    public async Task<IList<TDto>> GetAsync<TDto>(Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class {
        var query = QueryBuilder(GetAll.Select(entityToDto), sortOrder: sortOrder, pager: pager);

        return await RunQueryAsync(query, cancel).ConfigureAwait(false);
    }

    public async Task<IList<T>> GetAsync(Expression<Func<T, bool>> spec,
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) {
        var query = QueryBuilder(GetAll, spec, sortOrder: sortOrder, pager: pager);

        return spec is null
            ? Array.Empty<T>()
            : await RunQueryAsync(query, cancel).ConfigureAwait(false);
    }

    public async Task<IList<TDto>> GetAsync<TDto>(Expression<Func<TDto, bool>> spec,
        Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class {
        var query = QueryBuilder(GetAll.Select(entityToDto), spec, sortOrder: sortOrder, pager: pager);

        return spec is null
            ? Array.Empty<TDto>()
            : await RunQueryAsync(query, cancel).ConfigureAwait(false);
    }

    public async Task<IList<TDto>> GetAsync<TDto>(Expression<Func<T, bool>> spec,
        Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class {
        var query = QueryBuilder(GetAll.Where(spec).Select(entityToDto), sortOrder: sortOrder, pager: pager);

        return spec is null
            ? Array.Empty<TDto>()
            : await RunQueryAsync(query, cancel).ConfigureAwait(false);
    }

    public async Task<IList<T>> GetAsync(IEnumerable<TId> ids,
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) {
        return await GetAsync(spec: e => ids.Contains(e.Id), sortOrder, pager, cancel).ConfigureAwait(false);
    }

    public async Task<IList<TDto>> GetAsync<TDto>(IEnumerable<TId> ids, Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default)
        where TDto : class {
        return await GetAsync(spec: e => ids.Contains(e.Id), entityToDto, sortOrder, pager, cancel)
            .ConfigureAwait(false);
    }

    public async Task<IList<T>> GetAsync(
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default,
        params TId[] ids) {
        return await GetAsync(ids.AsEnumerable(), sortOrder, pager, cancel).ConfigureAwait(false);
    }

    public async Task<IList<TDto>> GetAsync<TDto>(Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default,
        params TId[] ids) where TDto : class {
        return await GetAsync(ids.AsEnumerable(), entityToDto, sortOrder, pager, cancel).ConfigureAwait(false);
    }

    public async Task<T?> GetAsync(TId id, CancellationToken cancel = default) {
        var query = GetAll.Where(e => e.Id.Equals(id));

        return await query.SingleOrDefaultAsync(cancel).ConfigureAwait(false);
    }

    public async Task<TDto?> GetAsync<TDto>(TId id, Expression<Func<T, TDto>> entityToDto,
        CancellationToken cancel = default) {
        var query = GetAll.Where(e => e.Id.Equals(id)).Select(entityToDto);

        return await query.SingleOrDefaultAsync(cancel).ConfigureAwait(false);
    }

    #endregion

    #region Подсчет сущностей

    public int Count() {
        return Entities.Count();
    }

    public async Task<int> CountAsync(CancellationToken cancel = default) {
        return await Entities.CountAsync(cancel).ConfigureAwait(false);
    }

    public int CountBy(Expression<Func<T, bool>>? spec = null) {
        return spec is null
            ? 0
            : QueryBuilder(Entities, spec).Count();
    }

    public async Task<int> CountByAsync(Expression<Func<T, bool>>? spec = null,
        CancellationToken cancel = default) {
        return spec is null
            ? 0
            : await QueryBuilder(Entities, spec).CountAsync(cancel).ConfigureAwait(false);
    }

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


    #region == Служебное

    #region Инициализация и очистка

    /// <summary>
    ///     Определяет немедленно загружаемые подчиненные сущности
    /// </summary>
    protected Func<IQueryable<T>, IIncludableQueryable<T, object>>? AutoInclude { get; set; }

    protected readonly DbContext Context;
    protected readonly DbSet<T> Entities;


    public Repository(DbContext dbContext) : this(dbContext, autoInclude: null) { }

    public Repository(DbContext dbContext,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? autoInclude = null) {
        Context = dbContext;
        Entities = dbContext.Set<T>();
        AutoInclude = autoInclude;


        LoadAll = QueryBuilder(Entities, include: AutoInclude);
        GetAll = QueryBuilder(LoadAll, options: QueryTune.ChangeTrackingIdentityResolution);
        // ReSharper disable once VirtualMemberCallInConstructor
        DefineQuery();
    }

    #endregion

    #region Предварительно сформулированные запросы

    protected IQueryable<T> LoadAll;
    protected IQueryable<T> GetAll;

    /// <summary>
    ///     Метод формирующий предварительно сформулированные запросы
    /// </summary>
    /// <remarks>
    ///     Метод должен быть переопределён в производных классах с обязательным вызовом метода базового класса
    /// </remarks>
    protected virtual void DefineQuery() { }

    #endregion

    #region Помощники

    /// <summary>
    ///     Отправляет запрос в центральный (базовый) склад на исполнение
    /// </summary>
    /// <param name="query">запрос</param>
    /// <param name="cancel">Токен отмены</param>
    /// <returns></returns>
    protected async Task<IList<TOut>> RunQueryAsync<TOut>(IQueryable<TOut> query, CancellationToken cancel = default) {
        return await query.ToListAsync(cancel).ConfigureAwait(false);
    }

    /// <summary>
    ///     Отправляет запрос, который должен вернуть большое количество сущностей,
    ///     в центральный (базовый) склад на исполнение
    /// </summary>
    /// <param name="query">запрос</param>
    /// <param name="cancel">Токен отмены</param>
    /// <returns></returns>
    protected async Task<ISet<TOut>>
        RunBigQueryAsync<TOut>(IQueryable<TOut> query, CancellationToken cancel = default) {
        return await query.ToHashSetAsync(cancel).ConfigureAwait(false);
    }

    /// <summary>
    ///     Формирует запрос для загрузки сущностей с центрального (базового) склада
    /// </summary>
    /// <typeparam name="TEntity">Тип запрашиваемой сущности</typeparam>
    /// <param name="query">запрос к центральному (базовому) складу</param>
    /// <param name="spec">Условие фильтрации</param>
    /// <param name="include">Выбрать немедленно загружаемые связанные сущности</param>
    /// <param name="sortOrder">Сортировать результат по</param>
    /// <param name="pager">Параметры постраничной загрузки</param>
    /// <param name="options">Не отслеживать изменения найденных сущностей</param>
    /// <returns><see cref="IQueryable{T}" />></returns>
    /// <remarks>Источник: https://github.com/arch/UnitOfWork/blob/master/src/UnitOfWork/Repository.cs</remarks>
    [Pure]
    protected static IQueryable<TEntity> QueryBuilder<TEntity>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? spec = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sortOrder = null,
        QueryTune options = QueryTune.None,
        (uint pageNum, uint pageSize)? pager = null)
        where TEntity : class {
        query = options switch {
            var opt when opt.HasFlag(QueryTune.ChangeTrackingEnable)
                => query.AsTracking(),
            var opt when opt.HasFlag(QueryTune.ChangeTrackingIdentityResolution)
                => query.AsNoTrackingWithIdentityResolution(),
            var opt when opt.HasFlag(QueryTune.ChangeTrackingDisable)
                => query.AsNoTracking(),
            _ => query
        };
        query = options switch {
            var opt when opt.HasFlag(QueryTune.QuerySingle) => query.AsSingleQuery(),
            var opt when opt.HasFlag(QueryTune.QuerySplit) => query.AsSplitQuery(),
            _ => query
        };
        if (options.HasFlag(QueryTune.GlobalFilterDisable))
            query = query.IgnoreQueryFilters();

        if (include is not null)
            query = include(query);
        if (spec is not null)
            query = query.Where(spec);
        if (sortOrder is not null)
            query = sortOrder(query);

        if (pager is (var num, var size and > 0))
            query = query.Skip((int)(num * size)).Take((int)size);

        return query;
    }

    /// <summary>
    ///     Формирует запрос для загрузки сущностей с центрального (базового) склада
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности <see cref="Entity{TId}" /></typeparam>
    /// <typeparam name="TEntityId">Тип идентификатора сущности</typeparam>
    /// <param name="query">запрос к центральному (базовому) складу</param>
    /// <param name="spec">Условие фильтрации</param>
    /// <param name="include">Выбрать немедленно загружаемые связанные сущности</param>
    /// <param name="sortOrder">Сортировать результат по</param>
    /// <param name="pager">Параметры постраничной загрузки</param>
    /// <param name="options">Не отслеживать изменения найденных сущностей</param>
    /// <returns><see cref="IQueryable{T}" />></returns>
    /// <remarks>Источник: https://github.com/arch/UnitOfWork/blob/master/src/UnitOfWork/Repository.cs</remarks>
    [Pure]
    protected static IQueryable<TEntity> QueryBuilder<TEntity, TEntityId>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? spec = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sortOrder = null,
        QueryTune options = QueryTune.None,
        (uint pageNum, uint pageSize)? pager = null)
        where TEntity : Entity<TEntityId>
        where TEntityId : struct, IEquatable<TEntityId> {
        return QueryBuilder(query, spec, include, sortOrder, options, pager);
    }

    /// <summary>
    ///     Формирует запрос для загрузки сущностей с центрального (базового) склада
    /// </summary>
    /// <param name="query">запрос к центральному (базовому) складу</param>
    /// <param name="spec">Условие фильтрации</param>
    /// <param name="include">Выбрать немедленно загружаемые связанные сущности</param>
    /// <param name="sortOrder">Сортировать результат по</param>
    /// <param name="pager">Параметры постраничной загрузки</param>
    /// <param name="options">Не отслеживать изменения найденных сущностей</param>
    /// <returns><see cref="IQueryable{T}" />></returns>
    /// <remarks>Источник: https://github.com/arch/UnitOfWork/blob/master/src/UnitOfWork/Repository.cs</remarks>
    [Pure]
    protected static IQueryable<T> QueryBuilder(
        IQueryable<T> query,
        Expression<Func<T, bool>>? spec = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null,
        QueryTune options = QueryTune.None,
        (uint pageNum, uint pageSize)? pager = null) {
        return QueryBuilder<T, TId>(query, spec, include, sortOrder, options, pager);
    }

    #endregion

    [Flags]
    protected enum QueryTune {
        /// <summary>
        ///     По умолчанию
        /// </summary>
        None,

        /// <summary>
        ///     Отслеживать загружаемые сущности
        /// </summary>
        ChangeTrackingEnable = 1 << 0,

        /// <summary>
        ///     Сущность с одним ключом будет иметь один и тот же экземпляр
        /// </summary>
        ChangeTrackingIdentityResolution = 1 << 1,

        /// <summary>
        ///     Не отслеживать загруженные сущности
        /// </summary>
        ChangeTrackingDisable = 1 << 2,

        /// <summary>
        ///     Загружать связанные сущности в одном запросе
        /// </summary>
        QuerySingle = 1 << 3,

        /// <summary>
        ///     Сделать несколько запросов связанных сущностей
        /// </summary>
        QuerySplit = 1 << 4,

        /// <summary>
        ///     Отключить фильтры уровня модели
        /// </summary>
        GlobalFilterDisable = 1 << 5
    }

    #endregion
}