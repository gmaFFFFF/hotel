using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.tests.BusinessLogic.Fixtures;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestBusinessCommandDbHandler() : BusinessCommandDbHandler<DbHandlerCommand, DbHandlerEvent,
    BusinessEntity, int, Repo, DbHandlerStatus,
    DbHandlerStatus>(new Repo()) {
    protected override IList<DbHandlerEvent> PackResultToEvent(IList<DbHandlerStatus> result) {
        throw new NotImplementedException();
    }
}