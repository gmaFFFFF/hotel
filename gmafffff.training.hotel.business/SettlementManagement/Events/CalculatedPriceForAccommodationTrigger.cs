using gmafffff.training.hotel.business.SettlementManagement.Commands;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.SettlementManagement.Events;

/// <summary>
///     Рассчитана цена за проживание
/// </summary>
public record CalculatedPriceForAccommodationTrigger(AccommodationReport<Guid> Report, MoveOutCommand Command)
    : TriggerEvent(Command);