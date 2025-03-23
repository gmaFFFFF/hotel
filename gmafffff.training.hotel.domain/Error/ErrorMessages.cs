using gmafffff.starterKit.AppError;

namespace gmafffff.training.hotel.domain.Error;

/// <summary>
///     Локализованные сообщения об ошибках
/// </summary>
public class ErrorMessages : IErrorMessage<ErrorCode> {
    public const string Ru = IErrorMessage<ErrorCode>.Ru;
    public const string En = IErrorMessage<ErrorCode>.En;

    // Словарь, где каждому языку соответствует словарь [код ошибки] = "Локализованное сообщение"
    public Dictionary<string, Dictionary<ErrorCode, string>>
        Messages { get; set; } = new() {
        [Ru] = new Dictionary<ErrorCode, string> {
            [ErrorCode.ValidationRoomNumberNo] = "Помещению необходим уникальный номер",
            [ErrorCode.ValidationRoomCapacityNo] = "Необходимо указать вместимость номера",
            [ErrorCode.ValidationPersonNameNo] = "Необходимо указать имя",
            [ErrorCode.ValidationSurNameNo] = "Необходимо указать фамилию"
        },
        [En] = new Dictionary<ErrorCode, string> {
            [ErrorCode.ValidationRoomNumberNo] = "The room needs a unique number",
            [ErrorCode.ValidationRoomCapacityNo] = "It is necessary to specify the capacity of the number",
            [ErrorCode.ValidationPersonNameNo] = "It is necessary to enter the name",
            [ErrorCode.ValidationSurNameNo] = "It is necessary to enter the sur name"
        }
    };
}