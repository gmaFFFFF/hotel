using gmafffff.starterKit.BusinessLogic;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestBusinessCommandDbHandler: BusinessCommandDbHandler<TestCommand,TestEvent,int> {
    protected override Task<Fin<Unit>> RunActionAsync(CancellationToken cancel = default) {
        throw new NotImplementedException();
    }

    protected override void PackResultToEvent() {
        throw new NotImplementedException();
    }
}