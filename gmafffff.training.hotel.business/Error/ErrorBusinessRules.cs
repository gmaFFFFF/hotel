namespace gmafffff.training.hotel.business.Error;

/// <summary>
///     Единый перечень нарушений бизнес-правил
/// </summary>
public enum ErrorBusinessRules {
    None,
    // № номера повторяется
    RuleRoomNumberRepeat = 0xd796,  // CRC16 gmafffff.training.hotel.business.Error.ErrorBusinessRules
    // номер занят
    RuleRoomBusy,
    // персона проживает в отеле
    RulePersonLivesInHotel,
    // количество жильцов превышает вместимость номера
    RuleNumberVisitorsExceedCapacityRoom,
}