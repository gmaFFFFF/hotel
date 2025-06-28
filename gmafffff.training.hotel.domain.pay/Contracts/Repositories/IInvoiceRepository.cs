using gmafffff.starterKit.Domain;
using gmafffff.training.hotel.domain.pay.Model;

namespace gmafffff.training.hotel.domain.pay.Contracts.Repositories;

/// <summary>
///     Кладовка счетов
/// </summary>
public interface IInvoiceRepository : IRepository<Invoice, int>;