using gmafffff.starterKit.Validation;
using gmafffff.training.hotel.business.Error;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using Validot;

namespace gmafffff.training.hotel.business.SettlementManagement.Validations;

public class SettleInCommandSpec : LocalizedSpecification,
    ISpecificationHolder<SettleInCommand> {
    private readonly Specification<SettleInCommand> _settleInCommandSpec;

    public SettleInCommandSpec() {
        Specification<IEnumerable<Guid>> visitors = s => s
            .NotEmptyCollection()
            .WithErrorCode(ErrorValidation.ValidationNotVisitors);

        Specification<DateOnly> departureDatePlanned = s => s
            .AsConverted(
                convert: d => d.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local),
                specification: d => d.AfterOrEqualTo(DateTime.Today))
            .WithErrorCode(ErrorValidation.ValidationDepartureDatePlanned);

        _settleInCommandSpec = s => s
            .Member(memberSelector: m => m.Visitors, visitors)
            .Member(memberSelector: m => m.DepartureDatePlanned, specification: s => s
                .Optional()
                .AsNullable(departureDatePlanned));
    }

    Specification<SettleInCommand> ISpecificationHolder<SettleInCommand>.Specification => _settleInCommandSpec;
}