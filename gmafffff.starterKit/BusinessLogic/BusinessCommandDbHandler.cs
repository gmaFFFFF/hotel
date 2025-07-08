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
    protected readonly ILogger<IBusinessCommandHandler<TCommand>> Logger =
        logger ?? NullLogger<IBusinessCommandHandler<TCommand>>.Instance;

    protected readonly TRepo Repository = repository;

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

        using var logScope = Logger.OpenBusinessCommandDbHandlerLogScope(command.MessageId);

        // Выполняем шаги последовательно, при условии успешного выполнения предыдущего шага и отсутствия отмены
        var steps =
            from loaded in LoadFromDb(Repository)
            from res in Act(loaded)
            from _ in DispatchDomainEvent()
            from count in SaveSeparate(res, Repository)
            from businessEvents in Pack(res)
            select businessEvents;

        var run = await steps
            .Run()
            .RunAsync(EnvIO.New(token: cancel))
            .ConfigureAwait(false);

        if (run.IsFail) return (Error)run;
        Result = run.IfFail(Array.Empty<BusinessEvent>()).ToArray();

        return Fin<IList<BusinessEvent>>.Succ(Result);

        // Функциональные обёртки для функций-шагов
        FinT<IO, IList<TLoad>> LoadFromDb(TRepo repo) {
            var loadAsync = async (EnvIO env) => {
                var loaded = await LoadAsync(repo, env.Token).ConfigureAwait(false);
                loaded.IfSucc(loaded => {
                    if (loaded.Any()) Logger.LoadDataSuccess();
                });
                loaded.IfFail(error => Logger.LoadDataFail(error));
                return loaded;
            };
            return IO.liftAsync(loadAsync);
        }

        FinT<IO, IList<TResult>> Act(IList<TLoad> loaded) {
            var actAsync = async (EnvIO env) => {
                var result = await RunActionAsync(loaded, env.Token).ConfigureAwait(false);
                result.IfSucc(_ => Logger.ComputeSuccess());
                result.IfFail(error => Logger.ComputeFail(error));
                return result;
            };
            return IO.liftAsync(actAsync);
        }

        FinT<IO, int> SaveDb(TRepo repo) {
            return IO.liftAsync(async env => {
                var count = await SaveAsync(repo, env.Token).ConfigureAwait(false);
                count.IfSucc(count => Logger.SaveSuccess(count));
                count.IfFail(error => Logger.SaveFail(error));
                return count;
            });
        }

        FinT<IO, int> SaveNotRequired() {
            return IO<int>.Lift(() => {
                Logger.SavingNotRequired();
                return 0;
            });
        }

        FinT<IO, int> SaveDbIf(bool isSave, TRepo repo) {
            return isSave ? SaveDb(repo) : SaveNotRequired();
        }

        FinT<IO, bool> BeforeSave(IList<TResult> res) {
            return IO.lift(() => OnBeforeSaving(res));
        }

        // Отдельные команды могут выполняться непосредственно в БД, поэтому операция сохранения может быть не нужна
        FinT<IO, int> SaveSeparate(IList<TResult> res, TRepo repo) {
            return IsSaveToDbSeparately
                ? BeforeSave(res).Bind(isSaveResult => SaveDbIf(isSaveResult, repo))
                : SaveNotRequired();
        }

        FinT<IO, IList<BusinessEvent>> Pack(IList<TResult> res) {
            return IO.lift(() => PackResultToEvent(res));
        }
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

        dispatch.IfSucc(_ => Logger.DispatchDomainEventsSuccess());
        dispatch.IfFail(error => Logger.DispatchDomainEventsFail(error));

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

internal static partial class BusinessCommandDbHandlerLog {
    /// <summary>
    ///     CRC16 для
    ///     <see cref="gmafffff.starterKit.BusinessLogic.BusinessCommandDbHandler{TCommand,TEntity,TId,TRepo,TLoad,TResult}" />
    /// </summary>
    public const int EventIdBase = 0x6098;

    private static readonly Func<ILogger, Guid, IDisposable?> OpenBusinessCommandDbHandlerLogScopeFunc =
        LoggerMessage.DefineScope<Guid>("Исполнение команды {CommandId}");

    public static IDisposable? OpenBusinessCommandDbHandlerLogScope(this ILogger logger, Guid commandId) {
        return OpenBusinessCommandDbHandlerLogScopeFunc(logger, commandId);
    }

    [LoggerMessage(EventId = EventIdBase + 1, Level = LogLevel.Trace, Message = "Данные загружены")]
    public static partial void LoadDataSuccess(this ILogger logger);

    [LoggerMessage(EventId = EventIdBase + 2, Level = LogLevel.Error, Message = "Ошибка загрузки данных: {@Error}")]
    public static partial void LoadDataFail(this ILogger logger, Error error);

    [LoggerMessage(EventId = EventIdBase + 3, Level = LogLevel.Trace, Message = "Вычисление завершено")]
    public static partial void ComputeSuccess(this ILogger logger);

    [LoggerMessage(EventId = EventIdBase + 4, Level = LogLevel.Error, Message = "Ошибка вычисления: {@Error}")]
    public static partial void ComputeFail(this ILogger logger, Error error);

    [LoggerMessage(EventId = EventIdBase + 5, Level = LogLevel.Trace, Message = "Сохранено записей: {count}")]
    public static partial void SaveSuccess(this ILogger logger, int count);

    [LoggerMessage(EventId = EventIdBase + 6, Level = LogLevel.Error, Message = "Ошибка сохранения: {@Error}")]
    public static partial void SaveFail(this ILogger logger, Error error);

    [LoggerMessage(EventId = EventIdBase + 7, Level = LogLevel.Trace, Message = "Отдельно сохранение не проводилось")]
    public static partial void SavingNotRequired(this ILogger logger);

    [LoggerMessage(EventId = EventIdBase + 8, Level = LogLevel.Trace, Message = "Обработаны доменные события")]
    public static partial void DispatchDomainEventsSuccess(this ILogger logger);

    [LoggerMessage(EventId = EventIdBase + 9, Level = LogLevel.Error,
        Message = "Обработка доменных событий провалена: {@Error}")]
    public static partial void DispatchDomainEventsFail(this ILogger logger, Error error);
}