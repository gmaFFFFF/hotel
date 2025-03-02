using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Бизнес правило
/// </summary>
public interface IBusinessRule<in TCommand>
    where TCommand : BusinessCommand {
    Enum ErrorCode { get; }

    /// <summary>
    ///     Выполняется ли бизнес-правило
    /// </summary>
    Task<bool> IsSatisfiedAsync(TCommand command, CancellationToken cancel = default);
}