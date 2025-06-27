using System.ComponentModel;
using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Преобразует <see cref="TriggerEvent" /> в список команд, которые нужно выполнить
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ITriggerEventToCommandTranslator {
    /// <summary>
    ///     Преобразовать <paramref name="trigger" /> в команды для выполнения
    /// </summary>
    /// <param name="trigger">событие-сигнал</param>
    IEnumerable<BusinessCommand> Translate(TriggerEvent trigger);
}

/// <summary>
///     Преобразует <typeparamref name="TTrigger" /> типа <see cref="TriggerEvent" /> в список команд, которые нужно
///     выполнить
/// </summary>
/// <typeparam name="TTrigger">Тип события-сигнала</typeparam>
public interface ITriggerEventToCommandTranslator<in TTrigger> : ITriggerEventToCommandTranslator
    where TTrigger : TriggerEvent {
    IEnumerable<BusinessCommand> ITriggerEventToCommandTranslator.Translate(TriggerEvent trigger) {
        return Translate((TTrigger)trigger);
    }

    /// <summary>
    ///     Преобразовать <paramref name="trigger" /> в команды для выполнения
    /// </summary>
    /// <param name="trigger">событие-сигнал</param>
    IEnumerable<BusinessCommand> Translate(TTrigger trigger);
}