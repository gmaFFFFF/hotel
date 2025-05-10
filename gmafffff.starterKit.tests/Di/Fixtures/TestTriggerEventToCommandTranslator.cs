using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestTriggerEventToCommandTranslator : ITriggerEventToCommandTranslator<TestTriggerEvent> {
    public IEnumerable<BusinessCommand> Translate(TestTriggerEvent trigger) {
        throw new NotImplementedException();
    }
}