using gmafffff.starterKit.BusinessLogic;
using gmafffff.starterKit.Messaging.Crud;
using gmafffff.training.hotel.business.InvoiceManagement.Commands;
using gmafffff.training.hotel.domain.pay.Contracts.Repositories;
using gmafffff.training.hotel.domain.pay.Model;
using Microsoft.Extensions.Logging;

namespace gmafffff.training.hotel.business.InvoiceManagement.Handlers;

public class AddInvoiceCommandHandler(
    IInvoiceRepository repo,
    IServiceProvider serviceProvider,
    ILogger<AddInvoiceCommandHandler>? logger = null)
    : BusinessCommandDbHandler<AddInvoiceCommand,
        Invoice, int, IInvoiceRepository,
        Unit, Invoice>(repo, serviceProvider, logger: logger) {
    protected override Task<Fin<IList<Invoice>>>
        RunActionAsync(IList<Unit> loaded, CancellationToken cancel = default) {
        repo.Add(Command.New);
        return Task.FromResult(Fin<IList<Invoice>>.Succ([Command.New]));
    }

    protected override IList<BusinessEvent> PackResultToEvent(IList<Invoice> result) {
        return result
            .Select(r => new CreatedBusinessEvent<int>(r.Id, Command))
            .Cast<BusinessEvent>()
            .ToArray();
    }
}