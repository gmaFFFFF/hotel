using gmafffff.starterKit.Messaging;
using LanguageExt;
using LanguageExt.Common;
using Light.GuardClauses;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Устанавливает типовой порядок выполнения команды, связанной с записью в БД
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TEvent"></typeparam>
/// <typeparam name="TResult"></typeparam>
public abstract class BusinessCommandDbHandler<TCommand, TEvent, TResult>(bool isSaveToDbSeparately = true) :
    IBusinessCommandHandler<TCommand, TEvent>
    where TCommand : BusinessCommand
    where TEvent : BusinessEvent {
    /// <summary>
    ///     Предварительный результат, несохранённый в БД
    /// </summary>
    protected IList<TResult> PreliminaryResult = [];

    /// <summary>
    ///     Выполнение команды происходит отдельно от сохранения её результата в БД?
    /// </summary>
    public bool IsSaveToDbSeparately { get; } = isSaveToDbSeparately;

    /// <summary>
    ///     Последняя обработанная команда
    /// </summary>
    public TCommand? LastCommand { get; private set; }

    /// <summary>
    ///     Результат успешного выполнения команды
    /// </summary>
    public IList<TEvent>? LastResult { get; protected set; }

    /// <summary>
    ///     Нужно ли отправить изменения в БД
    /// </summary>
    /// <remarks>
    ///     Изменяется обработчиком события <see cref="BeforeSaving" />
    /// </remarks>
    private bool IsSaveResult { get; set; } = true;

    public virtual async Task<Fin<IList<TEvent>>> ExecuteAsync(TCommand command,
        CancellationToken cancel = default) {
        LastCommand = command.MustNotBeNull();
        PreliminaryResult = [];
        LastResult = null;

        // --- Запись шагов (внутренних вызов функций) в функциональном стиле 
        // исключила дублирование проверок результата предыдущего шага и отмены

        // Функциональные обёртки для функций-шагов
        var load = FinT<IO, Unit>.LiftIO(IO.liftAsync(async env => await LoadAsync(env.Token).ConfigureAwait(false)));
        var act = FinT<IO, Unit>.LiftIO(
            IO.liftAsync(async env => await RunActionAsync(env.Token).ConfigureAwait(false)));
        var nothing = FinT<IO, Unit>.Lift(Fin<Unit>.Succ(Unit.Default));
        // Сохранение в БД выполняется если не было отменено
        var saveDbIf = (bool isSave) => isSave
            ? FinT<IO, Unit>.LiftIO(IO.liftAsync(async env => await SaveAsync(env.Token).ConfigureAwait(false)))
            : nothing;
        var beforeSave = FinT<IO, Unit>.Lift(IO.lift(OnBeforeSaving));
        // Отдельные команды могут выполняться непосредственно в БД, поэтому операция сохранения вероятно не нужна
        var saveSeparate = IsSaveToDbSeparately
            ? beforeSave.Bind(_ => saveDbIf(IsSaveResult))
            : nothing;
        var pack = FinT<IO, Unit>.Lift(IO.lift(PackResultToEvent));

        // Выполняем шаги последовательно, при условии успешного выполнения предыдущего шага и отсутствия отмены
        var steps = from _1 in load
            from _2 in act
            from _3 in saveSeparate
            from _4 in pack
            select _1;

        var run = await steps.Run().RunAsync(EnvIO.New(token: cancel)).ConfigureAwait(false);
        if (run.IsFail) return (Error)run;

        var result = LastResult ?? throw new InvalidOperationException("Отсутствует результат");
        return Fin<IList<TEvent>>.Succ(result);
    }

    /// <summary>
    ///     Событие вызывается перед сохранением результата выполнения команды.
    ///     Актуально для команд, которые выполняются независимо от сохранения в бд <see cref="IsSaveToDbSeparately" />
    /// </summary>
    public event EventHandler<BeforeSavingEventArgs>? BeforeSaving;

    /// <summary>
    ///     Загрузить данные, необходимые для выполнения команды
    /// </summary>
    protected virtual Task<Fin<Unit>> LoadAsync(CancellationToken cancel = default) {
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }

    /// <summary>
    ///     Непосредственное выполнение команды и сохранить предварительный результат в <see cref="PreliminaryResult" />
    /// </summary>
    protected abstract Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default);

    /// <summary>
    ///     Действие, выполняемое перед сохранением результатов в БД
    /// </summary>
    /// <remarks>
    ///     Не уверен, что этот хук нужен, но захотелось иметь возможность вмешаться в процесс перед сохранением
    /// </remarks>
    /// <exception cref="InvalidOperationException"></exception>
    protected virtual void OnBeforeSaving() {
        IsSaveResult = true;

        var result = PreliminaryResult ??
                     throw new InvalidOperationException("Отсутствует предварительный результат");
        var arg = new BeforeSavingEventArgs(LastCommand!, result, IsSaveResult);

        BeforeSaving?.Invoke(this, arg);

        IsSaveResult = arg.IsSaveResult;
    }

    /// <summary>
    ///     Сохранить результат выполнения команды
    /// </summary>
    /// <remarks>
    ///     Не каждая команда поддерживает сохранение
    /// </remarks>
    protected virtual Task<Fin<Unit>> SaveAsync(CancellationToken cancel = default) {
        return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
    }

    /// <summary>
    ///     Упаковывает результат в событие и сохраняет в <see cref="LastResult" />
    /// </summary>
    protected abstract void PackResultToEvent();

    /// <summary>
    ///     Аргументы события, вызываемого перед сохранением результатов выполнения команды
    /// </summary>
    /// <param name="command"></param>
    public class BeforeSavingEventArgs(
        TCommand command,
        ICollection<TResult> result,
        bool isSaveResult = true) : EventArgs {
        /// <summary>
        ///     Предварительный результат выполнения команды
        /// </summary>
        public IList<TResult> PreliminaryResult = [..result];

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