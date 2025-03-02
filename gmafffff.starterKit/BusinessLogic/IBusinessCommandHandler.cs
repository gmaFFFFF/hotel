using gmafffff.starterKit.Messaging;
using LanguageExt;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Выполняет операции в системе, соответствующие поступившей команде
/// </summary>
/// <typeparam name="TCommand">
///     Объект, производный от <see cref="BusinessCommand" />, представляющий команду, которую необходимо
///     выполнить
/// </typeparam>
/// <typeparam name="TEvent">
///     Объект, производный от <see cref="BusinessEvent" />, представляющий изменения, которые произошли в
///     системе
/// </typeparam>
public interface IBusinessCommandHandler<in TCommand, TEvent>
    where TCommand : BusinessCommand
    where TEvent : BusinessEvent {
    /// <summary>
    ///     Выполнить команду
    /// </summary>
    Task<Fin<IList<TEvent>>> ExecuteAsync(TCommand command, CancellationToken cancel = default);
}