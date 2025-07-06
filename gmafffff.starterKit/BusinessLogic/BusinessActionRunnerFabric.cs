using gmafffff.starterKit.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace gmafffff.starterKit.BusinessLogic;

public record BusinessActionRunnerFabric(IServiceProvider ServiceProvider)
    : IBusinessActionRunnerFabric {
    public IBusinessActionRunner GetBusinessActionRunner(BusinessCommand cmd) {
        var runnerType = typeof(IBusinessActionRunner<>).MakeGenericType(cmd.GetType());
        return (IBusinessActionRunner)ServiceProvider.GetRequiredService(runnerType);
    }
}