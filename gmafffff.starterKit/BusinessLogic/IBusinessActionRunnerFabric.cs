using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.BusinessLogic;

/// <summary>
///     Чтобы не использовать ServiceLocator в потребляющем классе
/// </summary>
public interface IBusinessActionRunnerFabric {
    IBusinessActionRunner GetBusinessActionRunner(BusinessCommand cmd);
}