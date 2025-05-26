using System.Transactions;
using gmafffff.starterKit.AppError;
using gmafffff.starterKit.Messaging;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Validot;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Проверяет команду <see cref="BusinessCommand" /> на соответствие формальным требованиями
///     и при условии соблюдения бизнес-правил <see cref="IBusinessConstraintCheck{TCommand}" />
///     отправляет её на исполнение, запуская команды,
///     соответствующие (<see cref="ITriggerEventToCommandTranslator{TTrigger}" />)
///     сигнальным событиям <see cref="TriggerEvent" />.
/// </summary>
public class BusinessActionRunner<TCommand>(IServiceProvider serviceProvider, ILogger? logger = null)
    : IBusinessActionRunner<TCommand>
    where TCommand : BusinessCommand {
    /// <summary>
    ///     Журнал
    /// </summary>
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    /// <summary>
    ///     Контейнер DI
    /// </summary>
    protected readonly IServiceProvider ServiceProvider = serviceProvider;

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
    ///     Установка в true повышает производительность, т.к. правила проверяются параллельно, а не последовательно
    /// </remarks>
    public bool ContinueCheckBusinessConstraintsAfterFirstError { get; set; } = true;

    /// <summary>
    ///     Выполнить команду
    /// </summary>
    public virtual async Task<Fin<IList<BusinessEvent>>> Execute(TCommand command,
        CancellationToken cancel = default) {
        using var _ = _logger.BeginScope("На исполнение поступила {@BusinessCommand}", command);

        var steps =
            from _1 in Validate(command)
            from _2 in CheckConstraint(command)
            from primaryEvents in Handle(command)
            from events in ProcessTriggerEvents(primaryEvents)
            select events;

        try {
            using var transaction = new TransactionScope(TransactionScopeOption.Required);
            var result = await steps.Run().RunAsync(EnvIO.New(token: cancel)).ConfigureAwait(false);
            result.IfSucc(_ => transaction.Complete());
            return result;
        }
        catch (OperationCanceledException) {
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
            return IO.liftAsync(async env => {
                var handler = ServiceProvider.GetRequiredService<IBusinessCommandHandler<TCommand>>();

                var result = await handler.ExecuteAsync(cmd, env.Token).ConfigureAwait(false);

                result.IfSucc(e => _logger.LogTrace("Результат исполнения команды: {@Events}", e));
                result.IfFail(e => _logger.LogTrace("Невозможно выполнить команду: {@Errors}", e));

                return result;
            });
        }
    }

    /// <summary>
    ///     Соответствует ли команда требованиям формальной корректности
    /// </summary>
    /// <returns>Возвращает true если формальных ошибок не выявлено</returns>
    protected Validation<Error, Unit> IsFormalValid(TCommand command) {
        var validator = ServiceProvider.GetService<IValidator<TCommand>>();

        if (validator is null)
            return Unit.Default;

        var result = validator.IsValid(command);

        _logger.LogTrace("Команда прошла форматно-логический контроль: {IsValid}", result);

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
            error += await ServiceProvider.GetServices<IBusinessConstraintCheck<TCommand>>()
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
            foreach (var constraint in ServiceProvider.GetServices<IBusinessConstraintCheck<TCommand>>()) {
                cancel.ThrowIfCancellationRequested();
                var test = await CheckConstraint(constraint, command, _logger).RunAsync(EnvIO.New(token: cancel));

                if (test.IsEmpty) continue;

                error += AppErrorHelper.NewError(constraint.ErrorCode);
                break;
            }

        _logger.LogTrace("Бизнес-ограничения соблюдаются: {IsSatisfied}", error.IsEmpty);

        return error.IsEmpty
            ? Unit.Default
            : error;


        static IO<bool> CallCheck(IBusinessConstraintCheck<TCommand> constraint, TCommand command, ILogger logger) {
            return IO.liftAsync(
                async env => {
                    var result = await constraint.IsSatisfiedAsync(command, env.Token).ConfigureAwait(false);
                    logger.LogTrace("Команда соответствует бизнес-ограничению {BusinessConstraintCheck}: {IsValid}",
                        constraint.ErrorCode, result);
                    return result;
                });
        }

        static IO<Error> CheckConstraint(IBusinessConstraintCheck<TCommand> constraint, TCommand command,
            ILogger logger) {
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

        var triggerResultsEvent = groupByTypeEvent.Where(group => group.Key)
            .SelectMany(e => e.AsEnumerable())
            .Cast<TriggerEvent>()
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
    /// <param name="trigger">событие—триггер</param>
    /// <returns></returns>
    private FinT<IO, IEnumerable<BusinessEvent>> RunTrigger(TriggerEvent trigger) {
        var commands = FindTriggerToCommandsTranslators(trigger)
            .Bind(translator => translator.Translate(trigger))
            .ToList();

        _logger.LogWarning("{@EventTrigger} сигнализирует о необходимости выполнить {@BusinessCommands}",
            trigger, commands);

        var runCommands = commands
            .AsIterable()
            .Traverse(RunCommand)
            .As()
            .Select(e => e.Flatten());

        // TODO: Может быть в команду нужно добавить ссылку на родительское событие?
        return runCommands;

        Iterable<ITriggerEventToCommandTranslator> FindTriggerToCommandsTranslators(TriggerEvent trigger) {
            var translatorType = typeof(ITriggerEventToCommandTranslator<>).MakeGenericType(trigger.GetType());
            var translators = ServiceProvider
                .GetServices(translatorType)
                .Cast<ITriggerEventToCommandTranslator>()
                .AsIterable();

            if (translators.IsEmpty())
                _logger.LogWarning("Не найден транслятор в команду для {@EventTrigger}", trigger);

            return translators;
        }

        FinT<IO, IList<BusinessEvent>> RunCommand(BusinessCommand cmd) {
            var runnerType = typeof(IBusinessActionRunner<>).MakeGenericType(cmd.GetType());
            var runner = (IBusinessActionRunner)ServiceProvider.GetRequiredService(runnerType);

            var execute = (BusinessCommand cmd) =>
                IO.liftAsync(async env => await runner.Execute(cmd, env.Token).ConfigureAwait(false));
            var result = FinT<IO, IList<BusinessEvent>>.LiftIO(execute(cmd));
            return result;
        }
    }
}