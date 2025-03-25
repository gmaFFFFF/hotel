using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestBusinessRule : IBusinessRule<BusinessCommand> {
    public Enum ErrorCode { get; set; }

    public Task<bool> IsSatisfiedAsync(BusinessCommand command, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }
}