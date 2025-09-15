using gmafffff.starterKit.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.BusinessLogic;

public record BusinessActionRunnerFactory(IServiceProvider ServiceProvider) {
    public virtual IBusinessActionRunner GetBusinessActionRunner(BusinessCommand cmd) {
        var runnerType = typeof(IBusinessActionRunner<>).MakeGenericType(cmd.GetType());
        return (IBusinessActionRunner)ServiceProvider.GetRequiredService(runnerType);
    }
}