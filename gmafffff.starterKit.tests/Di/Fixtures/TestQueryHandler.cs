using gmafffff.starterKit.BusinessLogic;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestQueryHandler : IQueryHandler<TestQuery, TestDto> {
    public async Task<Fin<IList<TestDto>>> RunQueryAsync(TestQuery query, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }
}