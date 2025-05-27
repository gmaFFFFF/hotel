using LanguageExt;

namespace gmafffff.starterKit.Domain.Events;

public interface IDomainEventDispatcher {
    /// <summary>
    ///     Обработать накопленные события предметной области
    /// </summary>
    Task<Fin<Unit>> DispatchAsync(CancellationToken cancel = default);
}