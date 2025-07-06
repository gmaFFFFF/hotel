using gmafffff.starterKit.Domain;
using gmafffff.starterKit.Domain.Events;
using gmafffff.starterKit.Messaging;
using LanguageExt;
using LanguageExt.Common;
using Light.GuardClauses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Устанавливает типовой порядок выполнения команды, связанной с записью в БД
/// </summary>
/// <typeparam name="TCommand">Исполняемая бизнес-команда</typeparam>
/// <typeparam name="TRepo">Оперативный склад, используемый для запроса</typeparam>
/// <typeparam name="TEntity">Сущность — корень агрегата</typeparam>
/// <typeparam name="TId">Идентификатор сущности</typeparam>
/// <typeparam name="TLoad">Тип загружаемой сущности из хранилища</typeparam>
/// <typeparam name="TResult">Тип сущности, полученный в результате команды</typeparam>
public abstract class BusinessCommandDbHandler<
    TCommand,
    TEntity, TId, TRepo,
    TLoad, TResult>(
    TRepo repository,
    IDomainEventDispatcher domainEventDispatcher,
    bool isSaveToDbSeparately = true,
    ILogger<IBusinessCommandHandler<TCommand>>? logger = null) :
    IBusinessCommandHandler<TCommand>
    where TCommand : BusinessCommand
    where TEntity : Entity<TId>
    where TId : struct, IEquatable<TId>
    where TRepo : IRepository<TEntity, TId> {
    /// <summary>
    ///     Журнал
    /// </summary>
    private readonly ILogger<IBusinessCommandHandler<TCommand>> _logger =
        logger ?? NullLogger<IBusinessCommandHandler<TCommand>>.Instance;

    /// <summary>
    ///     Выполнение команды происходит отдельно от сохранения её результата в БД?
    /// </summary>
    public bool IsSaveToDbSeparately { get; } = isSaveToDbSeparately;

    /// <summary>
    ///     Последняя обработанная команда
    /// </summary>
    public TCommand Command { get; private set; } = null!;

    /// <summary>
    ///     Результат успешного выполнения команды
    /// </summary>
    public IList<BusinessEvent>? Result { get; protected set; }

    public virtual async Task<Fin<IList<BusinessEvent>>> ExecuteAsync(TCommand command,
        CancellationToken cancel = default) {
        Command = command.MustNotBeNull();
        Result = null;

        // --- Запись шагов (внутренних вызов функций) в функциональном стиле 
        // исключила дублирование проверок результата предыдущего шага и отмены

        // Функциональные обёртки для функций-шагов
        var load = (TRepo repo) =>
            FinT<IO, IList<TLoad>>.LiftIO(
                IO.liftAsync(async env => await LoadAsync(repo, env.Token).ConfigureAwait(false)));
        var act = (IList<TLoad> loaded) =>
            FinT<IO, IList<TResult>>.LiftIO(
                IO.liftAsync(async env => await RunActionAsync(loaded, env.Token).ConfigureAwait(false)));
        var nothing = FinT<IO, int>.Lift(Fin<int>.Succ(0));
        var saveDb = (TRepo repo) =>
            FinT<IO, int>.LiftIO(
                IO.liftAsync(async env => await SaveAsync(repo, env.Token).ConfigureAwait(false)));
        // Сохранение в БД выполняется если не было отменено
        var saveDbIf = (bool isSave, TRepo repo) => isSave ? saveDb(repo) : nothing;
        var beforeSave = (IList<TResult> res) =>
            FinT<IO, bool>.Lift(
                IO.lift(() => OnBeforeSaving(res)));
        // Отдельные команды могут выполняться непосредственно в БД, поэтому операция сохранения может быть не нужна
        var saveSeparate = (IList<TResult> res, TRepo repo) => IsSaveToDbSeparately
            ? beforeSave(res).Bind(isSaveResult => saveDbIf(isSaveResult, repo))
            : nothing;
        var pack = (IList<TResult> res) =>
            FinT<IO, IList<BusinessEvent>>.Lift(
                IO.lift(() => PackResultToEvent(res)));


        // Выполняем шаги последовательно, при условии успешного выполнения предыдущего шага и отсутствия отмены
        var steps =
            from loaded in load(repository)
            from res in act(loaded)
            from _ in DispatchDomainEvent()
            from count in saveSeparate(res, repository)
            from businessEvents in pack(res)
            select businessEvents;

        var run = await steps
            .Run()
            .RunAsync(EnvIO.New(token: cancel))
            .ConfigureAwait(false);

        if (run.IsFail) return (Error)run;
        Result = run.IfFail(Array.Empty<BusinessEvent>()).ToArray();

        return Fin<IList<BusinessEvent>>.Succ(Result);
    }

    /// <summary>
    ///     Загрузить данные, необходимые для выполнения команды
    /// </summary>
    protected virtual Task<Fin<IList<TLoad>>> LoadAsync(TRepo repo, CancellationToken cancel = default) {
        return Task.FromResult(Fin<IList<TLoad>>.Succ([]));
    }

    /// <summary>
    ///     Непосредственное выполнение команды
    /// </summary>
    protected virtual Task<Fin<IList<TResult>>> RunActionAsync(IList<TLoad> loaded,
        CancellationToken cancel = default) {
        return Task.FromResult(Fin<IList<TResult>>.Succ([]));
    }

    /// <summary>
    ///     Действие, выполняемое перед сохранением результатов в БД
    /// </summary>
    /// <remarks>
    ///     Не уверен, что этот хук нужен, но захотелось иметь возможность вмешаться в процесс перед сохранением
    /// </remarks>
    /// <exception cref="InvalidOperationException"></exception>
    protected virtual bool OnBeforeSaving(IList<TResult> result) {
        var arg = new BeforeSavingEventArgs(Command, result, isSaveResult: true);

        BeforeSaving?.Invoke(this, arg);

        return arg.IsSaveResult;
    }

    /// <summary>
    ///     Событие вызывается перед сохранением результата выполнения команды.
    ///     Актуально для команд, которые выполняются независимо от сохранения в бд <see cref="IsSaveToDbSeparately" />
    /// </summary>
    public event EventHandler<BeforeSavingEventArgs>? BeforeSaving;

    /// <summary>
    ///     Сохранить результат выполнения команды
    /// </summary>
    /// <remarks>
    ///     Не каждая команда поддерживает сохранение
    /// </remarks>
    protected virtual async Task<Fin<int>> SaveAsync(TRepo repo, CancellationToken cancel = default) {
        return await repo
            .SaveChangesAsync(cancel)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Упаковывает результат в событие и сохраняет в <see cref="Result" />
    /// </summary>
    protected abstract IList<BusinessEvent> PackResultToEvent(IList<TResult> result);

    private FinT<IO, Unit> DispatchDomainEvent() {
        var dispatch = FinT<IO, Unit>.LiftIO(IO.liftAsync(async env =>
            await domainEventDispatcher.DispatchAsync(env.Token).ConfigureAwait(false)));

        dispatch.IfSucc(_ => _logger.LogTrace("Успешно обработаны доменные события"));
        dispatch.IfFail(error => _logger.LogTrace("При обработке доменных ошибок возникли ошибки: {@Errors}", error));

        return dispatch;
    }

    /// <summary>
    ///     Аргументы события, вызываемого перед сохранением результатов выполнения команды
    /// </summary>
    /// <param name="command"></param>
    public class BeforeSavingEventArgs(
        TCommand command,
        IList<TResult> result,
        bool isSaveResult = true) : EventArgs {
        /// <summary>
        ///     Предварительный результат выполнения команды
        /// </summary>
        public IList<TResult> Result = result;

        /// <summary>
        ///     Выполняемая команда
        /// </summary>
        public TCommand Command { get; set; } = command;

        /// <summary>
        ///     Нужно ли отправить изменения в БД
        /// </summary>
        public bool IsSaveResult { get; set; } = isSaveResult;
    }
}