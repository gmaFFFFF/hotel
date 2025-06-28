using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.domain.pay.Model;

namespace gmafffff.training.hotel.business.InvoiceManagement.Commands;

/// <summary>
///     Добавить в систему новый счет на оплату
/// </summary>
public record AddInvoiceCommand(Invoice New) : CreateBusinessCommand<Invoice>(New);