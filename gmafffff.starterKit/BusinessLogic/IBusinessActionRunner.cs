using System.ComponentModel;
using gmafffff.starterKit.Messaging;
using LanguageExt;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Проверяет команду <see cref="BusinessCommand" /> на соответствие формальным требованиями
///     и при условии соблюдения бизнес-правил <see cref="IBusinessConstraintCheck{TCommand}" />
///     отправляет её на исполнение, запуская команды,
///     соответствующие (<see cref="ITriggerEventToCommandTranslator{TTrigger}" />)
///     сигнальным событиям <see cref="TriggerEvent" />.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IBusinessActionRunner {
    /// <summary>
    ///     Выполнить команду
    /// </summary>
    Task<Fin<IList<BusinessEvent>>> ExecuteAsync(BusinessCommand command, CancellationToken cancel = default);
}

/// <summary>
///     Проверяет команду <see cref="BusinessCommand" /> на соответствие формальным требованиями
///     и при условии соблюдения бизнес-ограничений <see cref="IBusinessConstraintCheck{TCommand}" />
///     отправляет её на исполнение, запуская команды,
///     соответствующие (<see cref="ITriggerEventToCommandTranslator{TTrigger}" />)
///     сигнальным событиям <see cref="TriggerEvent" />.
/// </summary>
public interface IBusinessActionRunner<in TCommand> : IBusinessActionRunner
    where TCommand : BusinessCommand {
    async Task<Fin<IList<BusinessEvent>>> IBusinessActionRunner.ExecuteAsync(BusinessCommand command,
        CancellationToken cancel) {
        return await ExecuteAsync((TCommand)command, cancel).ConfigureAwait(false);
    }

    /// <summary>
    ///     Выполнить команду
    /// </summary>
    Task<Fin<IList<BusinessEvent>>> ExecuteAsync(TCommand command, CancellationToken cancel = default);
}