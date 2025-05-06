using gmafffff.starterKit.AppError;
using gmafffff.starterKit.Messaging;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Validot;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Обеспечивает выполнение команд, при условии соблюдения бизнес-правил
/// </summary>
public class BusinessActionRunner<TCommand>(IServiceProvider serviceProvider)
    : IBusinessActionRunner
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

    Task<Fin<IList<BusinessEvent>>> IBusinessActionRunner.Execute(BusinessCommand command,
        CancellationToken cancel) {
        return Execute((TCommand)command, cancel);
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
            from events in FinT<IO, IList<BusinessEvent>>.LiftIO(handle(command))
            select events;

        try {
            return await steps.Run().RunAsync(EnvIO.New(token: cancel)).ConfigureAwait(false);
        }
        catch (OperationCanceledException) {
            return AppErrorHelper.NewError(AppErrorCode.OperationCancel);
        }
        catch (DbUpdateConcurrencyException concurrencyException) {
            return AppErrorHelper.NewError(AppErrorCode.DbConcurrentWrite, concurrencyException);
        }
    }
}