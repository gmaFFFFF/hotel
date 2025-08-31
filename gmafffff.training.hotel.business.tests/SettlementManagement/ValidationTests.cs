using FluentAssertions;
using gmafffff.training.hotel.business.Error;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.business.SettlementManagement.Validations;
using gmafffff.training.hotel.business.tests.Fixtures;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Validot;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.SettlementManagement;

public partial class SettlementManagementTests {
    [TestSubject(typeof(SettleInCommandSpec))]
    public class Validation(ITestOutputHelper output) : TestContext(output) {
        public static TheoryData<SettleInCommand, Enum?> SettleInCommandSample {
            get {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var yesterday = today.AddDays(-1);
                return new TheoryData<SettleInCommand, Enum?> {
                    // Проверка на наличие посетителей
                    { new SettleInCommand(RoomId: default, [Guid.Empty]), null },
                    { new SettleInCommand(RoomId: default, []), ErrorValidation.ValidationNotVisitors },

                    // Проверка планируемой даты выезда
                    { new SettleInCommand(RoomId: default, [Guid.Empty], DepartureDatePlanned: null), null },
                    { new SettleInCommand(RoomId: default, [Guid.Empty], today), null }, {
                        new SettleInCommand(RoomId: default, [Guid.Empty], yesterday),
                        ErrorValidation.ValidationDepartureDatePlanned
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(SettleInCommandSample))]
        public void ValidateSettleInCommand(SettleInCommand command, Enum? errorCode) {
            // Arrange
            var validator = FakeServiceProvider.Instance.GetRequiredService<IValidator<SettleInCommand>>();

            // Act
            var result = validator.Validate(command);

            // Assert
            if (errorCode is null)
                result.AnyErrors.Should().BeFalse();
            else
                result.ToErrorCodes().Should().Contain(errorCode);
        }
    }
}