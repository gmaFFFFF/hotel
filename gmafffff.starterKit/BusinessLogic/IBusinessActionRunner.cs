using gmafffff.starterKit.Messaging;
using LanguageExt;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Запускает команду на исполнение
/// </summary>
public interface IBusinessActionRunner {
    Task<Fin<IList<BusinessEvent>>> Execute(BusinessCommand command, CancellationToken cancel = default);
}