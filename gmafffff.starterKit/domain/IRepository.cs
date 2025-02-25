using System.Collections.Immutable;
using System.Linq.Expressions;

namespace gmafffff.starterKit.Domain;

/// <summary>
///     Кладовая (оперативный склад) для специализированного хранения сущностей типа <typeparamref name="T" />
/// </summary>
/// <typeparam name="T">Тип основной единицы хранения (сущности)</typeparam>
/// <typeparam name="TId">Тип идентификатора сущности</typeparam>
public interface IRepository<T, in TId> : IDisposable
    where T : Entity<TId>
    where TId : struct, IEquatable<TId> {
    #region Поиск и загрузка

    /// <summary>
    ///     Принимает на учёт все сущности из центрального (базисного) склада и выдаёт их
    /// </summary>
    /// <returns><see cref="IImmutableList{T}" /> всех сущностей</returns>
    Task<IImmutableList<T>> LoadAllAsync(CancellationToken cancel = default);

    /// <summary>
    ///     Отпускает все сущности из центрального (базисного) склада трансфером, не принимая их к себе на учёт
    /// </summary>
    /// <returns><see cref="IImmutableList{T}" /> всех сущностей</returns>
    Task<IImmutableList<T>> GetAllDetachAsync(CancellationToken cancel = default);


    /// <summary>
    ///     Ищет искомые сущности по идентификаторам в кладовой и выдает их.
    ///     Отсутствующие сущности предварительно запрашивает из центрального (базисного) склада и принимает их на учёт
    /// </summary>
    /// <param name="ids">Список идентификаторов сущностей для поиска</param>
    /// <returns><see cref="IImmutableList{T}" /> найденных сущностей</returns>
    IImmutableList<T> Find(IEnumerable<TId> ids);

    /// <summary>
    ///     Ищет искомые сущности по идентификаторам в кладовой и выдает их.
    ///     Отсутствующие сущности предварительно запрашивает из центрального (базисного) склада и принимает их на учёт
    /// </summary>
    /// <param name="ids">Список идентификаторов для поиска</param>
    /// <returns><see cref="IImmutableList{T}" /> найденных сущностей</returns>
    IImmutableList<T> Find(params TId[] ids);

    /// <summary>
    ///     Ищет единственную сущность, по идентификатору <paramref name="id" /> в кладовой и выдает её.
    ///     Отсутствующую сущность предварительно запрашивает из центрального (базисного) склада и принимает её на учёт
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Найденная сущность или null, если она не найдена в кладовой и центральном (базисном) складе</returns>
    T? Find(TId id);


    /// <summary>
    ///     Асинхронно ищет искомые сущности по идентификаторам в кладовой и выдает их.
    ///     Отсутствующие сущности предварительно запрашивает из центрального (базисного) склада и принимает их на учёт
    /// </summary>
    /// <param name="ids">Список идентификаторов для поиска</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IImmutableList{T}" /> найденных сущностей</returns>
    Task<IImmutableList<T>> FindAsync(IEnumerable<TId> ids, CancellationToken cancel = default);

    /// <summary>
    ///     Асинхронно ищет искомые сущности по идентификаторам в кладовой и выдает их.
    ///     Отсутствующие сущности предварительно запрашивает из центрального (базисного) склада и принимает их на учёт
    /// </summary>
    /// <param name="ids">Список идентификаторов для поиска</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IImmutableList{T}" /> найденных сущностей</returns>
    Task<IImmutableList<T>> FindAsync(CancellationToken cancel = default, params TId[] ids);

    /// <summary>
    ///     Асинхронно ищет единственную сущность, по идентификатору <paramref name="id" /> в кладовой и выдает её.
    ///     Отсутствующую сущность предварительно запрашивает из центрального (базисного) склада и принимает её на учёт
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns>Найденная сущность или null, если она не найдена в кладовой и центральном (базисном) складе</returns>
    Task<T?> FindAsync(TId id, CancellationToken cancel = default);


    /// <summary>
    ///     Принимает на учёт сущности, соответствующие спецификации <paramref name="spec" />,
    ///     из центрального (базисного) склада и выдаёт их
    /// </summary>
    /// <param name="spec">Особенности, которым должны соответствовать отпускаемые сущности, в форме предиката</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт пустой список</remarks>
    /// <returns><see cref="IImmutableList{T}" /> принятых на учёт сущностей</returns>
    IImmutableList<T> Load(Expression<Func<T, bool>>? spec = null);

    /// <summary>
    ///     Принимает на учёт сущности, с определёнными <paramref name="ids" />,
    ///     из центрального (базисного) склада и выдаёт их
    /// </summary>
    /// <param name="ids">Список идентификаторов отпускаемых сущностей</param>
    /// <returns><see cref="IImmutableList{T}" /> принятых на учёт сущностей</returns>
    IImmutableList<T> Load(IEnumerable<TId> ids);

    /// <summary>
    ///     Принимает на учёт сущности, с определёнными <paramref name="ids" />,
    ///     из центрального (базисного) склада и выдаёт их
    /// </summary>
    /// <param name="ids">Список идентификаторов отпускаемых сущностей</param>
    /// <returns><see cref="IImmutableList{T}" /> принятых на учёт сущностей</returns>
    IImmutableList<T> Load(params TId[] ids);

    /// <summary>
    ///     Принимает на учёт сущность, с определённым <paramref name="id" />,
    ///     из центрального (базисного) склада и выдаёт её
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Принятая на учёт сущность или null, если она не найдена в центральном (базисном) складе</returns>
    T? Load(TId id);


    /// <summary>
    ///     Асинхронно принимает на учёт сущности, соответствующие спецификации <paramref name="spec" />,
    ///     из центрального (базисного) склада и выдаёт их
    /// </summary>
    /// <param name="spec">Особенности, которым должны соответствовать отпускаемые сущности, в форме предиката</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт пустой список</remarks>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IImmutableList{T}" /> принятых на учёт сущностей</returns>
    Task<IImmutableList<T>> LoadAsync(Expression<Func<T, bool>>? spec = null,
        CancellationToken cancel = default);

    /// <summary>
    ///     Асинхронно принимает на учёт сущности, с определёнными <paramref name="ids" />,
    ///     из центрального (базисного) склада и выдаёт их
    /// </summary>
    /// <param name="ids">Список идентификаторов отпускаемых сущностей</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IImmutableList{T}" /> принятых на учёт сущностей</returns>
    Task<IImmutableList<T>> LoadAsync(IEnumerable<TId> ids, CancellationToken cancel = default);

    /// <summary>
    ///     Асинхронно принимает на учёт сущности, с определёнными <paramref name="ids" />,
    ///     из центрального (базисного) склада и выдаёт их
    /// </summary>
    /// <param name="ids">Список идентификаторов отпускаемых сущностей</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IImmutableList{T}" /> принятых на учёт сущностей</returns>
    Task<IImmutableList<T>> LoadAsync(CancellationToken cancel = default, params TId[] ids);

    /// <summary>
    ///     Принимает на учёт сущность, с определённым <paramref name="id" />,
    ///     из центрального (базисного) склада и выдаёт её
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns>Принятая на учёт сущность или null, если она не найдена в центральном (базисном) складе</returns>
    Task<T?> LoadAsync(TId id, CancellationToken cancel = default);

    /// <summary>
    ///     Ищет единственную сущность, по идентификатору <paramref name="id" /> в кладовой и выдает её.
    ///     Отсутствующую сущность предварительно запрашивает из центрального (базисного) склада и принимает её на учёт
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Найденная сущность или null, если она не найдена в кладовой и центральном (базисном) складе</returns>
    T? this[TId id] { get; }

    /// <summary>
    ///     Принимает на учёт сущности, соответствующие спецификации <paramref name="spec" />,
    ///     из центрального (базисного) склада и выдаёт их
    /// </summary>
    /// <param name="spec">Особенности, которым должны соответствовать отпускаемые сущности, в форме предиката</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт пустой список</remarks>
    /// <returns><see cref="IImmutableList{T}" /> принятых на учёт сущностей</returns>
    IImmutableList<T> this[Expression<Func<T, bool>>? spec] { get; }


    /// <summary>
    ///     Асинхронно принимает на учёт господствующие сущности, соответствующие спецификации <paramref name="spec" />,
    ///     из центрального (базисного) склада и выдаёт их. Подчинённые сущности не загружаются
    /// </summary>
    /// <param name="spec">Особенности, которым должны соответствовать отпускаемые сущности, в форме предиката</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то вернёт пустой список</remarks>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <returns><see cref="IImmutableList{T}" /> принятых на учёт сущностей</returns>
    Task<IImmutableList<T>> LoadOnlyRootAsync(Expression<Func<T, bool>>? spec = null,
        CancellationToken cancel = default);

    #endregion


    #region Добавление

    /// <summary>
    ///     Планирует постановку сущности на учёт в центральном (базовом) складе
    /// </summary>
    /// <param name="entity">Добавляемая сущность</param>
    void Add(T entity);

    /// <summary>
    ///     Планирует постановку сущностей на учёт в центральном (базовом) складе
    /// </summary>
    /// <param name="entities">Добавляемые сущности</param>
    void Add(IEnumerable<T> entities);

    /// <summary>
    ///     Планирует постановку составной части сущности на учёт в центральном (базовом) складе
    /// </summary>
    /// <param name="entity">Добавляемая сущность</param>
    void Add(object entity);

    /// <summary>
    ///     Планирует постановку составных частей сущностей на учёт в центральном (базовом) складе
    /// </summary>
    /// <param name="entities">Добавляемые сущности</param>
    void Add(IEnumerable<object> entities);

    #endregion


    #region Удаление

    /// <summary>
    ///     Планирует снятие сущности с учёта в центральном (базовом) складе
    /// </summary>
    /// <param name="entity">Удаляемая сущность</param>
    void Delete(T entity);

    /// <summary>
    ///     Планирует снятие сущностей с учёта в центральном (базовом) складе
    /// </summary>
    /// <param name="entities">Удаляемые сущности</param>
    void Delete(IEnumerable<T> entities);

    /// <summary>
    ///     Планирует снятие сущности с учёта в центральном (базовом) складе по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор удаляемой сущности</param>
    void Delete(TId id);

    /// <summary>
    ///     Планирует снятие сущностей с учёта в центральном (базовом) складе по идентификаторам
    /// </summary>
    /// <param name="ids">Список идентификаторов удаляемых сущностей</param>
    void Delete(IEnumerable<TId> ids);

    /// <summary>
    ///     Планирует снятие составной части сущности с учёта в центральном (базовом) складе
    /// </summary>
    /// <param name="entity">Удаляемая сущность</param>
    void Delete<TChild, TChildId>(TChild entity)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId>;

    /// <summary>
    ///     Планирует снятие составных частей сущностей с учёта в центральном (базовом) складе
    /// </summary>
    /// <param name="entities">Удаляемые сущности</param>
    void Delete<TChild, TChildId>(IEnumerable<TChild> entities)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId>;

    /// <summary>
    ///     Планирует снятие составной части сущности с учёта в центральном (базовом) складе по идентификатору
    /// </summary>
    /// <param name="entityId">Идентификатор удаляемой сущности</param>
    void Delete<TChild, TChildId>(TChildId entityId)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId>;

    /// <summary>
    ///     Планирует снятие составной части сущности с учёта в центральном (базовом) складе по идентификаторам
    /// </summary>
    /// <param name="entityIds">Список идентификаторов удаляемых сущностей</param>
    void Delete<TChild, TChildId>(IEnumerable<TChildId> entityIds)
        where TChild : Entity<TChildId>
        where TChildId : struct, IEquatable<TChildId>;


    /// <summary>
    ///     Немедленно снимает сущности с учёта в центральном (базовом) складе по спецификации
    /// </summary>
    /// <param name="spec">Спецификация удаляемых сущностей в форме предиката</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то ничего не удалит</remarks>
    /// <returns>Общее число снятых с учёта сущностей</returns>
    int DeleteBulk(Expression<Func<T, bool>>? spec);

    /// <summary>
    ///     Асинхронно немедленно снимает сущности с учёта в центральном (базовом) складе по спецификации
    /// </summary>
    /// <param name="spec">Спецификация удаляемых сущностей в форме предиката</param>
    /// <param name="cancel"><see cref="CancellationToken" /> отмены операции</param>
    /// <remarks>Если спецификация <paramref name="spec" /> не задана (null), то ничего не удалит</remarks>
    /// <returns>Общее число снятых с учёта сущностей</returns>
    Task<int> DeleteBulkAsync(Expression<Func<T, bool>>? spec, CancellationToken cancel = default);

    #endregion


    #region Замена всех свойств сущности

    /// <summary>
    ///     Заменить все свойства сущности
    /// </summary>
    /// <param name="entity">Обновленная сущность</param>
    void Update(T entity);

    /// <summary>
    ///     Заменить все свойства подчинённой сущности
    /// </summary>
    /// <param name="entity">Обновленная сущность</param>
    void Update(object entity);

    #endregion


    #region Характеристики

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


    #region Сохранение

    /// <summary>
    ///     Синхронизирует сущности центрального (базового) склада в соответствии с изменениями,
    ///     произошедшими с сущностями, учтёнными в кладовой
    /// </summary>
    /// <returns>Количество синхронизированных сущностей</returns>
    int SaveChanges();


    /// <summary>
    ///     Синхронизирует сущности центрального (базового) склада в соответствии с изменениями,
    ///     произошедшими с сущностями, учтёнными в кладовой
    /// </summary>
    /// <returns>Количество синхронизированных сущностей</returns>
    Task<int> SaveChangesAsync(CancellationToken cancel = default);

    #endregion
}