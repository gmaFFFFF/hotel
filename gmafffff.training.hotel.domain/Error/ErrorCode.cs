namespace gmafffff.training.hotel.domain.Error;

/// <summary>
///     Единый перечень ошибок домена приложения
/// </summary>
public enum ErrorCode {
    None,
    ValidationRoomNumberNo = 0x8203,  // CRC16 gmafffff.training.hotel.domain.Error.ErrorCode
    ValidationRoomCapacityNo,
    ValidationPersonNameNo,
    ValidationSurNameNo
}