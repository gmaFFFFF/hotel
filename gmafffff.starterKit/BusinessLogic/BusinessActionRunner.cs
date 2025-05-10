using System.Transactions;
using gmafffff.starterKit.AppError;
using gmafffff.starterKit.Messaging;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Validot;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Проверяет команду <see cref="BusinessCommand" /> на соответствие формальным требованиями
///     и при условии соблюдения бизнес-правил <see cref="IBusinessRule{TCommand}"/> отправляет её на исполнение,
///     запуская команды, соответствующие (<see cref="ITriggerEventToCommandTranslator{TTrigger}"/>)
///     сигнальным событиям <see cref="TriggerEvent" />.
/// </summary>
public class BusinessActionRunner<TCommand>(IServiceProvider serviceProvider)
    : IBusinessActionRunner<TCommand>
    where TCommand : BusinessCommand {
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
    ///     Продолжить проверку на соответствие бизнес правилам
    ///     после выявления первого несоответствия
    /// </summary>
    /// <remarks>
    ///     Установка в true повышает производительность, т.к. правила проверяются параллельно, а не последовательно
    /// </remarks>
    public bool ContinueValidateBusinessRulesAfterFirstError { get; set; } = true;

    /// <summary>
    ///     Выполнить команду
    /// </summary>
    public virtual async Task<Fin<IList<BusinessEvent>>> Execute(TCommand command,
        CancellationToken cancel = default) {
        var handler = ServiceProvider.GetRequiredService<IBusinessCommandHandler<TCommand>>();

        var validate = (TCommand cmd) => IsFormalValid(cmd).ToFin();
        var violateRule = (TCommand cmd) => IO.liftAsync(async env =>
            (await IsBusinessRulesSatisfyAsync(cmd, env.Token).ConfigureAwait(false)).ToFin());
        var handle = (TCommand cmd) =>
            IO.liftAsync(async env => await handler.ExecuteAsync(cmd, env.Token).ConfigureAwait(false));

        var steps =
            from _1 in FinT<IO, Unit>.Lift(validate(command))
            from _2 in FinT<IO, Unit>.LiftIO(violateRule(command))
            from primaryEvents in FinT<IO, IList<BusinessEvent>>.LiftIO(handle(command))
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
    }

    /// <summary>
    ///     Соответствует ли команда требованиям формальной корректности
    /// </summary>
    /// <returns>Возвращает true если формальных ошибок не выявлено</returns>
    protected Validation<Error, Unit> IsFormalValid(TCommand command) {
        var validator = ServiceProvider.GetService<IValidator<TCommand>>();

        if (validator is null)
            return Unit.Default;

        return validator.IsValid(command)
            ? Unit.Default
            : IncludeFormalValidationError
                ? validator.Validate(command).ToExceptedError()
                : AppErrorHelper.NewError(AppErrorCode.ValidationGeneral);
    }

    /// <summary>
    ///     Выполнены ли бизнес требования
    /// </summary>
    /// <returns>Возвращает true или коды ошибок</returns>
    /// <remarks> Исключение, возникшее при проверке бизнес-правила считается его невыполнением</remarks>
    protected async Task<Validation<Error, Unit>> IsBusinessRulesSatisfyAsync(TCommand command,
        CancellationToken cancel = default) {
        var error = Error.Empty;

        if (ContinueValidateBusinessRulesAfterFirstError)
            // В функциональном стиле уродливый код сократился в 2 раза, параллельность исполнения обеспечена «из коробки»
            error += await ServiceProvider.GetServices<IBusinessRule<TCommand>>()
                // Загруженный список правил трансформируем в аналог IEnumerable
                .AsIterable()
                // Запускаем проверку каждого правила так же как и в Select, но IO станет внешней монадой,
                // а не внутренней: IO<Iterable<Error>>, а не Iterable<IO<Error>>>
                .Traverse(rule => CheckRule(rule, command))
                // Собираем ошибки в одну ошибку
                .Map(errors => errors.Fold())
                // Запуск
                .RunAsync(EnvIO.New(token: cancel))
                .ConfigureAwait(false);

        else
            foreach (var rule in ServiceProvider.GetServices<IBusinessRule<TCommand>>()) {
                cancel.ThrowIfCancellationRequested();
                var test = await CheckRule(rule, command).RunAsync(EnvIO.New(token: cancel));

                if (test.IsEmpty) continue;

                error += AppErrorHelper.NewError(rule.ErrorCode);
                break;
            }

        return error.IsEmpty
            ? Unit.Default
            : error;


        static IO<bool> CallRule(IBusinessRule<TCommand> rule, TCommand command) {
            return IO.liftAsync(async env => await rule.IsSatisfiedAsync(command, env.Token).ConfigureAwait(false));
        }

        static IO<Error> CheckRule(IBusinessRule<TCommand> rule, TCommand command) {
            return (from test in CallRule(rule, command)
                    let er = test
                        ? Error.Empty
                        : AppErrorHelper.NewError(rule.ErrorCode)
                    select er)
                // Если обработка бизнес-правил вызвала исключение, то считаем, что проверка не пройдена
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
        var commands = FindTriggerToCommandsTranslators(trigger).Bind(translator => translator.Translate(trigger));

        var runCommand = commands
            .Traverse(RunCommand)
            .As()
            .Select(e => e.Flatten());

        return runCommand;


        Iterable<ITriggerEventToCommandTranslator> FindTriggerToCommandsTranslators(TriggerEvent trigger) {
            var translatorType = typeof(ITriggerEventToCommandTranslator<>).MakeGenericType(trigger.GetType());
            var translators = ServiceProvider.GetServices(translatorType).Cast<ITriggerEventToCommandTranslator>();
            return translators.AsIterable();
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