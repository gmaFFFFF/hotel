using System.Collections.Immutable;
using System.Linq.Expressions;

namespace gmafffff.starterKit.Domain;

public interface IRepositoryReadOnly<T, in TId>
    where T : Entity<TId>
    where TId : struct, IEquatable<TId> {
    #region Выгрузка данных потребителю без постановки на учет в оперативном складе

    /// <summary>
    ///     Отпускает все сущности из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <returns><see cref="IImmutableList{T}" /> всех сущностей</returns>
    Task<IList<T>> GetAsync(
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default);

    /// <summary>
    ///     Отпускает Dto всех сущностей из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <typeparam name="TDto">Обменный формат сущности</typeparam>
    /// <param name="entityToDto">Проекция сущности в Dto</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Постраничная загрузка</param>
    /// <param name="cancel"></param>
    /// <returns><see cref="IImmutableList{T}" /> всех сущностей</returns>
    Task<IList<TDto>> GetAsync<TDto>(Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class;

    /// <summary>
    ///     Асинхронно отпускает сущности, соответствующие спецификации <paramref name="spec" />,
    ///     из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <param name="spec">Особенности, которым должны соответствовать отпускаемые сущности, в форме предиката</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт пустой список</remarks>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Постраничная загрузка</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IList{T}" /> выданных сущностей</returns>
    Task<IList<T>> GetAsync(Expression<Func<T, bool>> spec,
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default);

    /// <summary>
    ///     Асинхронно отпускает Dto сущностей, соответствующих спецификации <paramref name="spec" />,
    ///     из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <typeparam name="TDto">Обменный формат сущности</typeparam>
    /// <param name="spec">Особенности, которым должны соответствовать отпускаемые DTO, в форме предиката</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт пустой список</remarks>
    /// <param name="entityToDto">Проекция сущности в Dto</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Постраничная загрузка</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IList{T}" /> выданных сущностей</returns>
    Task<IList<TDto>> GetAsync<TDto>(Expression<Func<TDto, bool>> spec, Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class;

    /// <summary>
    ///     Асинхронно отпускает Dto сущностей, соответствующих спецификации <paramref name="spec" />,
    ///     из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <typeparam name="TDto">Обменный формат сущности</typeparam>
    /// <param name="spec">Особенности, которым должны соответствовать отпускаемые сущности, в форме предиката</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт пустой список</remarks>
    /// <param name="entityToDto">Проекция сущности в Dto</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Постраничная загрузка</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <remarks>
    ///     Метод имеет компромиссный интерфейс: фильтрует по сущности, но сортирует по DTO,
    ///     так как фильтрация по сущности, несмотря на нарушение инкапсуляции, даёт максимум гибкости,
    ///     в то же время, сортировка по DTO сделает результат предсказуемым для пользователя.
    /// </remarks>
    /// <returns><see cref="IList{T}" /> выданных сущностей</returns>
    Task<IList<TDto>> GetAsync<TDto>(Expression<Func<T, bool>> spec, Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class;

    /// <summary>
    ///     Асинхронно отпускает сущности, с определёнными <paramref name="ids" />,
    ///     из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <param name="ids">Список идентификаторов отпускаемых сущностей</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Постраничная загрузка</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IList{T}" /> выданных сущностей</returns>
    Task<IList<T>> GetAsync(IEnumerable<TId> ids,
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default);

    /// <summary>
    ///     Асинхронно отпускает Dto сущностей, с определёнными <paramref name="ids" />,
    ///     из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <typeparam name="TDto">Обменный формат сущности</typeparam>
    /// <param name="ids">Список идентификаторов отпускаемых сущностей</param>
    /// <param name="entityToDto">Проекция сущности в Dto</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Постраничная загрузка</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IList{T}" /> выданных сущностей</returns>
    Task<IList<TDto>> GetAsync<TDto>(IEnumerable<TId> ids, Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default) where TDto : class;

    /// <summary>
    ///     Асинхронно отпускает сущности, с определёнными <paramref name="ids" />,
    ///     из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <param name="ids">Список идентификаторов отпускаемых сущностей</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Постраничная загрузка</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IList{T}" /> выданных сущностей</returns>
    Task<IList<T>> GetAsync(
        Func<IQueryable<T>, IOrderedQueryable<T>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default,
        params TId[] ids);

    /// <summary>
    ///     Асинхронно отпускает Dto сущностей, с определёнными <paramref name="ids" />,
    ///     из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <typeparam name="TDto">Обменный формат сущности</typeparam>
    /// <param name="entityToDto">Проекция сущности в Dto</param>
    /// <param name="ids">Список идентификаторов отпускаемых сущностей</param>
    /// <param name="sortOrder">Порядок сортировки</param>
    /// <param name="pager">Постраничная загрузка</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IList{T}" /> выданных сущностей</returns>
    Task<IList<TDto>> GetAsync<TDto>(Expression<Func<T, TDto>> entityToDto,
        Func<IQueryable<TDto>, IOrderedQueryable<TDto>>? sortOrder = null, (uint pageNum, uint pageSize)? pager = null,
        CancellationToken cancel = default,
        params TId[] ids) where TDto : class;

    /// <summary>
    ///     Асинхронно отпускает сущность, с определённым <paramref name="id" />,
    ///     из центрального (базисного) склада трансфером, не принимая её к себе на учёт
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns>Выданная сущность или null, если она не найдена в центральном (базисном) складе</returns>
    Task<T?> GetAsync(TId id, CancellationToken cancel = default);

    /// <summary>
    ///     Асинхронно отпускает Dto сущности, с определённым <paramref name="id" />,
    ///     из центрального (базисного) склада трансфером, не принимая её к себе на учёт
    /// </summary>
    /// <typeparam name="TDto">Обменный формат сущности</typeparam>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="entityToDto">Проекция сущности в Dto</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns>Выданная сущность или null, если она не найдена в центральном (базисном) складе</returns>
    Task<TDto?> GetAsync<TDto>(TId id, Expression<Func<T, TDto>> entityToDto, CancellationToken cancel = default);

    #endregion

    #region Подсчет сущностей

    /// <summary>
    ///     Возвращает число сущностей в центральном (базовом) складе
    /// </summary>
    int Count();

    /// <summary>
    ///     Асинхронно возвращает число сущностей в центральном (базовом) складе
    /// </summary>
    Task<int> CountAsync(CancellationToken cancel = default);

    /// <summary>
    ///     Подсчитывает количество сущностей в центральном (базовом) складе,
    ///     соответствующих <paramref name="spec" />
    /// </summary>
    /// <param name="spec">Спецификация включаемых в подсчет сущностей</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт 0</remarks>
    /// <returns>Число сущностей, соответствующих спецификации <paramref name="spec" /></returns>
    int CountBy(Expression<Func<T, bool>>? spec = null);

    /// <summary>
    ///     Асинхронно подсчитывает количество сущностей в центральном (базовом) складе,
    ///     соответствующих <paramref name="spec" />
    /// </summary>
    /// <param name="spec">Спецификация включаемых в подсчет сущностей</param>
    /// <param name="cancel">Токен отмены</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт 0</remarks>
    /// <returns>Число сущностей, соответствующих спецификации <paramref name="spec" /></returns>
    Task<int> CountByAsync(Expression<Func<T, bool>>? spec = null,
        CancellationToken cancel = default);

    #endregion
}