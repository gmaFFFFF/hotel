using FluentAssertions;
using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Utils;
using gmafffff.training.hotel.business.InvoiceManagement.Translators;
using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.business.SettlementManagement.Events;
using gmafffff.training.hotel.business.tests.Fixtures;
using gmafffff.training.hotel.domain.Model;
using gmafffff.training.hotel.domain.pay.Contracts.Repositories;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace gmafffff.training.hotel.business.tests.SettlementManagement;

public partial class SettlementManagementTests {
    [TestSubject(typeof(CalculatedPriceForAccommodationTriggerToCommandTranslator))]
    public class PaymentIntegration(ITestOutputHelper output) : TestContext(output) {
        /// <summary>
        ///     После выезда из номера выставляется счет на оплату
        /// </summary>
        [Fact]
        public async Task AfterMoveOutCreateInvoice() {
            // Arrange
            var invoiceRepository = Scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
            var room = FakeHotel.Hotel.Rooms
                .Where(Room<Guid>.IsFreeRoom.Not().Compile())
                .First(room => room.Visit.ArrivalDate < DateOnly.FromDateTime(DateTime.Today));
            var command = new MoveOutCommand(room.Id);
            var runner = Scope.ServiceProvider.GetRequiredService<IBusinessActionRunner<MoveOutCommand>>();

            // Act
            var res = await runner.Execute(command);

            // Assert
            res.IsSucc.Should().BeTrue();
            var report = res.Map(be => be.OfType<MovedOutEvent>().ToArray()).SuccSpan()[0][0].Report;
            var invoice = (await invoiceRepository.GetAsync()).Single();
            invoice.Price.Should().Be(report.Price);
        }
    }
}