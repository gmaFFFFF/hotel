namespace gmafffff.training.hotel.business.Error;

/// <summary>
///     Единый перечень нарушений бизнес-ограничений
/// </summary>
public enum ErrorBusinessConstraintCheck {
    None,

    // № номера повторяется
    CheckRoomNumberRepeat = 0x6c26, // CRC16 gmafffff.training.hotel.business.Error.ErrorBusinessConstraintCheck

    // номер занят
    CheckRoomBusy,

    // персона проживает в отеле
    CheckPersonLivesInHotel,

    // количество жильцов превышает вместимость номера
    CheckNumberVisitorsExceedCapacityRoom
}