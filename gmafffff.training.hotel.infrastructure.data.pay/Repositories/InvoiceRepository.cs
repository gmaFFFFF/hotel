using gmafffff.starterKit.EntityFrameworkCore;
using gmafffff.training.hotel.domain.pay.Contracts.Repositories;
using gmafffff.training.hotel.domain.pay.Model;
using gmafffff.training.hotel.infrastructure.data.pay.Sessions;

namespace gmafffff.training.hotel.infrastructure.data.pay.Repositories;

public class InvoiceRepository(InvoiceDbContext dbContext)
    : Repository<Invoice, int>(dbContext),
        IInvoiceRepository;