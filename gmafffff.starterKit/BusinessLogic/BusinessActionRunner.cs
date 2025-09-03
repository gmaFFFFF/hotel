using System.Transactions;
using gmafffff.starterKit.AppError;
using gmafffff.starterKit.Messaging;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Validot;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Оболочка пусковика бизнес-действий <see cref="ExecuteAsync"/>.
///     Проверяет бизнес-команду <see cref="BusinessCommand" />
///     на соответствие формальным требованиям <paramref name="validator"/>
///     и при условии соблюдения бизнес-правил <paramref name="businessConstraintChecks"/>
///     отправляет её на исполнение.
///     Если в результате обработки команды возникают сигнальные события <see cref="TriggerEvent" />,
///     то генерируются (<see cref="ITriggerEventToCommandTranslator{TTrigger}" />) новые бизнес-команды
///     и также отправляются на исполнение.
/// </summary>
/// <param name="validator">Форматно-логический контроль (формальная проверка) бизнес-команды</param>
/// <param name="businessConstraintChecks">Бизнес-ограничения, ограничивающие запуск бизнес-команд</param>
/// <param name="businessCommandHandler">Обработчик бизнес-команды</param>
/// <param name="businessActionRunnerFabric">Фабрика пусковиков бизнес-действий</param>
/// <param name="triggerEventToCommandTranslatorFabric">Фабрика преобразователей триггеров в бизнес-команды</param>
/// <param name="logger">Журнал</param>
/// <typeparam name="TCommand">Тип бизнес-команды</typeparam>
public class BusinessActionRunner<TCommand>(
    IBusinessCommandHandler<TCommand> businessCommandHandler,
    BusinessActionRunnerFabric businessActionRunnerFabric,
    IEnumerable<IBusinessConstraintCheck<TCommand>> businessConstraintChecks,
    TriggerEventToCommandTranslatorFabric triggerEventToCommandTranslatorFabric,
    IValidator<TCommand>? validator = null,
    ILogger<BusinessActionRunner<TCommand>>? logger = null)
    : IBusinessActionRunner<TCommand>
    where TCommand : BusinessCommand {
    /// <summary>
    ///     Журнал
    /// </summary>
    private readonly ILogger<BusinessActionRunner<TCommand>> _logger =
        logger ?? NullLogger<BusinessActionRunner<TCommand>>.Instance;

    /// <summary>
    ///     Собрать выявленные ошибки формальной корректности модели (не рекомендуется),
    ///     или просто констатировать наличие некорректной модели
    /// </summary>
    public bool IncludeFormalValidationError { get; set; } = false;

    /// <summary>
    ///     Продолжить проверку на соответствие бизнес-ограничениям
    ///     после выявления первого несоответствия
    /// </summary>
    /// <remarks>
    ///     Установка в true повышает производительность, так как правила проверяются параллельно, а не последовательно
    /// </remarks>
    public bool ContinueCheckBusinessConstraintsAfterFirstError { get; set; } = true;

    /// <summary>
    ///     Выполнить команду
    /// </summary>
    public virtual async Task<Fin<IList<BusinessEvent>>> ExecuteAsync(TCommand command,
        CancellationToken cancel = default) {
        var steps =
            from _1 in Validate(command)
            from _2 in CheckConstraint(command)
            from primaryEvents in Handle(command)
            from events in ProcessTriggerEvents(primaryEvents)
            select events;

        using var transaction = new TransactionScope(TransactionScopeOption.Required);
        using var _ = _logger.OpenBusinessActionRunnerLogScope(command);
        try {
            var result = await steps.Run().RunAsync(EnvIO.New(token: cancel)).ConfigureAwait(false);
            result.IfSucc(_ => transaction.Complete());
            return result;
        }
        catch (OperationCanceledException) {
            _logger.HandleCommandCanceled();
            return AppErrorHelper.NewError(AppErrorCode.OperationCancel);
        }
        catch (DbUpdateConcurrencyException concurrencyException) {
            return AppErrorHelper.NewError(AppErrorCode.DbConcurrentWrite, concurrencyException);
        }

        FinT<IO, Unit> Validate(TCommand command) {
            return IsFormalValid(command).ToFin();
        }

        FinT<IO, Unit> CheckConstraint(TCommand cmd) {
            return IO.liftAsync(async env =>
                (await IsBusinessConstraintsSatisfyAsync(cmd, env.Token).ConfigureAwait(false)).ToFin());
        }

        FinT<IO, IList<BusinessEvent>> Handle(TCommand cmd) {
            var execute = FinT<IO, IList<BusinessEvent>>.LiftIO(IO.liftAsync(async env =>
                await businessCommandHandler.ExecuteAsync(cmd, env.Token).ConfigureAwait(false)));

            execute.IfSucc(events => _logger.HandleCommandSuccess(events));
            execute.IfFail(error => _logger.HandleCommandFail(error));

            return execute;
        }
    }

    /// <summary>
    ///     Соответствует ли команда требованиям формальной корректности
    /// </summary>
    /// <returns>Возвращает true если формальных ошибок не выявлено</returns>
    protected Validation<Error, Unit> IsFormalValid(TCommand command) {
        if (validator is null)
            return Unit.Default;

        var result = validator.IsValid(command);

        _logger.ValidateCommand(result);

        return result
            ? Unit.Default
            : IncludeFormalValidationError
                ? validator.Validate(command).ToExceptedError()
                : AppErrorHelper.NewError(AppErrorCode.ValidationGeneral);
    }

    /// <summary>
    ///     Выполнены ли бизнес-ограничения
    /// </summary>
    /// <returns>Возвращает true или коды ошибок</returns>
    /// <remarks> Исключение, возникшее при проверке бизнес-ограничения считается его невыполнением</remarks>
    protected async Task<Validation<Error, Unit>> IsBusinessConstraintsSatisfyAsync(TCommand command,
        CancellationToken cancel = default) {
        var error = Error.Empty;

        if (ContinueCheckBusinessConstraintsAfterFirstError)
            // В функциональном стиле уродливый код сократился в 2 раза, параллельность исполнения обеспечена «из коробки»
            error += await businessConstraintChecks
                // Загруженный список правил трансформируем в аналог IEnumerable
                .AsIterable()
                // Запускаем проверку каждого правила так же как и в Select, но IO станет внешней монадой,
                // а не внутренней: IO<Iterable<Error>>, а не Iterable<IO<Error>>>
                .Traverse(constraint => CheckConstraint(constraint, command, _logger))
                // Собираем ошибки в одну ошибку
                .Map(errors => errors.Fold())
                // Запуск
                .RunAsync(EnvIO.New(token: cancel))
                .ConfigureAwait(false);

        else
            foreach (var constraint in businessConstraintChecks) {
                cancel.ThrowIfCancellationRequested();
                var test = await CheckConstraint(constraint, command, _logger).RunAsync(EnvIO.New(token: cancel));

                if (test.IsEmpty) continue;

                error += AppErrorHelper.NewError(constraint.ErrorCode);
                break;
            }

        _logger.CheckAllBusinessConstraints(error.IsEmpty);

        return error.IsEmpty
            ? Unit.Default
            : error;


        static IO<bool> CallCheck(IBusinessConstraintCheck<TCommand> constraint, TCommand command,
            ILogger<BusinessActionRunner<TCommand>> logger) {
            return IO.liftAsync(async env => {
                var result = await constraint.IsSatisfiedAsync(command, env.Token).ConfigureAwait(false);
                logger.CheckBusinessConstraint(constraint.ErrorCode, result);
                return result;
            });
        }

        static IO<Error> CheckConstraint(IBusinessConstraintCheck<TCommand> constraint, TCommand command,
            ILogger<BusinessActionRunner<TCommand>> logger) {
            return (from test in CallCheck(constraint, command, logger)
                    let er = test
                        ? Error.Empty
                        : AppErrorHelper.NewError(constraint.ErrorCode)
                    select er)
                // Если обработка бизнес-ограничений вызвала исключение, то считаем, что проверка не пройдена
                .IfFail(x => x);
        }
    }

    /// <summary>
    ///     Заменяет события типа с <see cref="TriggerEvent" /> результатом выполнения связанных с ними команд
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    private FinT<IO, IList<BusinessEvent>> ProcessTriggerEvents(IList<BusinessEvent> events) {
        var groupByTypeEvent = events.GroupBy(@event => @event is TriggerEvent);

        var simpleEvent = groupByTypeEvent
            .Where(group => !group.Key)
            .SelectMany(e => e.AsEnumerable());

        var triggerEvents = groupByTypeEvent.Where(group => group.Key)
            .SelectMany(e => e.AsEnumerable())
            .Cast<TriggerEvent>()
            .ToArray();

        if (triggerEvents.Length != 0)
            _logger.EmitTriggers(triggerEvents);

        var triggerResultsEvent = triggerEvents
            .AsIterable()
            .Traverse(RunTrigger)
            .As()
            .Select(e => e.Flatten());

        return triggerResultsEvent
            .Select(newEvent => simpleEvent.Concat(newEvent))
            .Select(events => (IList<BusinessEvent>)events.ToList());
    }

    /// <summary>
    ///     Находит и запускает команды, связанные с <paramref name="trigger" />
    /// </summary>
    /// <param name="trigger">событие-триггер</param>
    /// <returns></returns>
    private FinT<IO, IEnumerable<BusinessEvent>> RunTrigger(TriggerEvent trigger) {
        var commands = FindTriggerToCommandsTranslators(trigger)
            .Bind(translator => translator.Translate(trigger))
            .ToList();

        _logger.TranslateTriggerToCommands(trigger, commands);

        var runCommands = commands
            .AsIterable()
            .Traverse(RunCommand)
            .As()
            .Select(e => e.Flatten());

        // TODO: Может быть в команду нужно добавить ссылку на родительское событие?
        return runCommands;

        Iterable<ITriggerEventToCommandTranslator> FindTriggerToCommandsTranslators(TriggerEvent trigger) {
            var translators = triggerEventToCommandTranslatorFabric
                .GetTranslators(trigger)
                .AsIterable();

            if (translators.IsEmpty())
                _logger.TriggerTranslatorNotFound(trigger);

            return translators;
        }

        FinT<IO, IList<BusinessEvent>> RunCommand(BusinessCommand cmd) {
            var runner = businessActionRunnerFabric.GetBusinessActionRunner(cmd);

            var execute = (BusinessCommand cmd) =>
                IO.liftAsync(async env => await runner.ExecuteAsync(cmd, env.Token).ConfigureAwait(false));
            var result = FinT<IO, IList<BusinessEvent>>.LiftIO(execute(cmd));
            return result;
        }
    }
}

internal static partial class BusinessActionRunnerLog {
    /// <summary>
    ///     CRC16 для <see cref="gmafffff.starterKit.BusinessLogic.BusinessActionRunner{TCommand}" />
    /// </summary>
    public const int EventIdBase = 0xff11;

    private static readonly Func<ILogger, BusinessCommand, IDisposable?> OpenBusinessActionRunnerLogScopeFunc =
        LoggerMessage.DefineScope<BusinessCommand>("Конвейер обработки бизнес команды {@BusinessCommand}");

    public static IDisposable? OpenBusinessActionRunnerLogScope(this ILogger logger, BusinessCommand command) {
        return OpenBusinessActionRunnerLogScopeFunc(logger, command);
    }

    [LoggerMessage(EventId = EventIdBase + 1, Level = LogLevel.Trace,
        Message = "Форматно-логический контроль команды: {IsValid}")]
    public static partial void ValidateCommand(this ILogger logger, bool isValid);

    [LoggerMessage(EventId = EventIdBase + 2, Level = LogLevel.Trace,
        Message = "Проверка бизнес-ограничения {BusinessConstraint} для команды: {IsSatisfied}")]
    public static partial void CheckBusinessConstraint(this ILogger logger, Enum businessConstraint, bool isSatisfied);

    [LoggerMessage(EventId = EventIdBase + 3, Level = LogLevel.Trace,
        Message = "Проверены все бизнес-ограничения: {IsSatisfied}")]
    public static partial void CheckAllBusinessConstraints(this ILogger logger, bool isSatisfied);

    [LoggerMessage(EventId = EventIdBase + 4, Level = LogLevel.Trace,
        Message = "Команда исполнена. Произошли события: {@Events}")]
    public static partial void HandleCommandSuccess(this ILogger logger, IList<BusinessEvent> events);

    [LoggerMessage(EventId = EventIdBase + 5, Level = LogLevel.Error,
        Message = "Ошибка исполнения команды: {@Error}")]
    public static partial void HandleCommandFail(this ILogger logger, Error error);

    [LoggerMessage(EventId = EventIdBase + 6, Level = LogLevel.Warning, Message = "Исполнение команды отменено")]
    public static partial void HandleCommandCanceled(this ILogger logger);

    [LoggerMessage(EventId = EventIdBase + 7, Level = LogLevel.Trace,
        Message = "Возникли пусковые события {@Triggers}")]
    public static partial void EmitTriggers(this ILogger logger, TriggerEvent[] triggers);

    [LoggerMessage(EventId = EventIdBase + 8, Level = LogLevel.Debug,
        Message = "Не найден транслятор в команду для {@Trigger}")]
    public static partial void TriggerTranslatorNotFound(this ILogger logger, TriggerEvent trigger);

    [LoggerMessage(EventId = EventIdBase + 9, Level = LogLevel.Trace,
        Message = "{@Trigger} сигнализирует о необходимости выполнить {@Commands}")]
    public static partial void TranslateTriggerToCommands(this ILogger logger, TriggerEvent trigger,
        IList<BusinessCommand> commands);
}