using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.pay.Model;

/// <summary>
///     Счет на оплату
/// </summary>
/// <remarks>
///     Цель добавления сущности — апробация обработки сигнальных событий
/// </remarks>
public class Invoice : Entity<int> {
    public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string Purpose { get; set; }
    public decimal Price { get; set; }
}