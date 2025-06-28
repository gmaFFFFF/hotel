using gmafffff.starterKit.BusinessLogic;
using gmafffff.training.hotel.business.InvoiceManagement.Commands;
using gmafffff.training.hotel.business.SettlementManagement.Events;
using gmafffff.training.hotel.domain.pay.Model;

namespace gmafffff.training.hotel.business.InvoiceManagement.Translators;

/// <summary>
///     Преобразует сигнальное событие в команду
/// </summary>
public class CalculatedPriceForAccommodationTriggerToCommandTranslator
    : ITriggerEventToCommandTranslator<CalculatedPriceForAccommodationTrigger> {
    public IEnumerable<BusinessCommand> Translate(CalculatedPriceForAccommodationTrigger trigger) {
        return [
            new AddInvoiceCommand(new Invoice {
                Purpose = $"Оплата услуги проживания № {trigger.Report.Id}",
                Price = trigger.Report.Price
            })
        ];
    }
}