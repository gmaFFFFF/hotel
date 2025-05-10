using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Проверяющий бизнес-ограничений
/// </summary>
public interface IBusinessConstraintCheck<in TCommand>
    where TCommand : BusinessCommand {
    Enum ErrorCode { get; }

    /// <summary>
    ///     Выполняется ли бизнес-ограничение
    /// </summary>
    Task<bool> IsSatisfiedAsync(TCommand command, CancellationToken cancel = default);
}