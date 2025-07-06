using gmafffff.starterKit.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.BusinessLogic;

public record TriggerEventToCommandTranslatorFabric(IServiceProvider ServiceProvider)
    : ITriggerEventToCommandTranslatorFabric {
    public IEnumerable<ITriggerEventToCommandTranslator> GetTranslators(TriggerEvent trigger) {
        var translatorType = typeof(ITriggerEventToCommandTranslator<>).MakeGenericType(trigger.GetType());
        return ServiceProvider
            .GetServices(translatorType)
            .Cast<ITriggerEventToCommandTranslator>();
    }
}