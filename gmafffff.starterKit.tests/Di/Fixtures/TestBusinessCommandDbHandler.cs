using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging;
using gmafffff.starterKit.tests.BusinessLogic.Fixtures;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestBusinessCommandDbHandler() : BusinessCommandDbHandler<DbHandlerCommand,
    BusinessEntity, int, Repo, DbHandlerStatus,
    DbHandlerStatus>(new Repo(), null!) {
    protected override IList<BusinessEvent> PackResultToEvent(IList<DbHandlerStatus> result) {
        throw new NotImplementedException();
    }
}