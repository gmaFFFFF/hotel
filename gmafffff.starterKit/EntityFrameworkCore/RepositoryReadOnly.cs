using System.Linq.Expressions;
using gmafffff.starterKit.Domain;
using JetBrains.Annotations;
using Light.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace gmafffff.starterKit.EntityFrameworkCore;

/// <summary>
///     Реализация <see cref="IRepositoryReadOnly{T,TId}" /> для EF Core
/// </summary>
public class RepositoryReadOnly<T, TId> : IRepositoryReadOnly<T, TId>
    where T : Entity<TId>
    where TId : struct, IEquatable<TId> {
    #region == Запросы

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

    #region == Служебное

    #region Инициализация и очистка

    /// <summary>
    ///     Определяет немедленно загружаемые подчиненные сущности
    /// </summary>
    protected Func<IQueryable<T>, IIncludableQueryable<T, object>>? AutoInclude { get; set; }

    protected readonly DbContext Context;
    protected readonly DbSet<T> Entities;

    /// <summary>
    ///     Запрос на отпуск сущностей через оперативный склад
    /// </summary>
    protected readonly IQueryable<T> GetAll;

    public RepositoryReadOnly(DbContext dbContext,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? autoInclude = null) {
        Context = dbContext.MustNotBeNull();
        Entities = dbContext.Set<T>();
        AutoInclude = autoInclude;

        GetAll = QueryBuilder(Entities, include: AutoInclude, options: QueryTune.ChangeTrackingIdentityResolution);
    }

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