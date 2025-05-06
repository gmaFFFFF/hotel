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
public interface IBusinessCommandHandler<in TCommand>
    where TCommand : BusinessCommand {
    /// <summary>
    ///     Выполнить команду
    /// </summary>
    Task<Fin<IList<BusinessEvent>>> ExecuteAsync(TCommand command, CancellationToken cancel = default);
}