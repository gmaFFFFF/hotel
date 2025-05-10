using gmafffff.starterKit.Messaging;
using LanguageExt;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Проверяет команду <see cref="BusinessCommand" /> на соответствие формальным требованиями
///     и при условии соблюдения бизнес-ограничений <see cref="IBusinessConstraintCheck{TCommand}"/> отправляет её на исполнение,
///     запуская команды, соответствующие (<see cref="ITriggerEventToCommandTranslator{TTrigger}"/>)
///     сигнальным событиям <see cref="TriggerEvent" />.
/// </summary>
public interface IBusinessActionRunner {
    /// <summary>
    ///     Выполнить команду
    /// </summary>
    Task<Fin<IList<BusinessEvent>>> Execute(BusinessCommand command, CancellationToken cancel = default);
}

/// <summary>
///     Проверяет команду <see cref="BusinessCommand" /> на соответствие формальным требованиями
///     и при условии соблюдения бизнес-ограничений <see cref="IBusinessConstraintCheck{TCommand}" /> отправляет её на исполнение,
///     запуская команды, соответствующие (<see cref="ITriggerEventToCommandTranslator{TTrigger}" />)
///     сигнальным событиям <see cref="TriggerEvent" />.
/// </summary>
public interface IBusinessActionRunner<in TCommand> : IBusinessActionRunner
    where TCommand : BusinessCommand {
    Task<Fin<IList<BusinessEvent>>> IBusinessActionRunner.Execute(BusinessCommand command,
        CancellationToken cancel) {
        return Execute((TCommand)command, cancel);
    }

    /// <summary>
    ///     Выполнить команду
    /// </summary>
    Task<Fin<IList<BusinessEvent>>> Execute(TCommand command, CancellationToken cancel = default);
}