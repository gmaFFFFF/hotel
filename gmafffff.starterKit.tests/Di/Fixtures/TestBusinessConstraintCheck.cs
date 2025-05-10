using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging;

namespace gmafffff.starterKit.tests.Di.Fixtures;

public class TestBusinessConstraintCheck : IBusinessConstraintCheck<BusinessCommand> {
    public Enum ErrorCode { get; set; }

    public Task<bool> IsSatisfiedAsync(BusinessCommand command, CancellationToken cancel = default) {
        throw new NotImplementedException();
    }
}