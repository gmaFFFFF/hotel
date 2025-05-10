using gmafffff.starterKit.AppError;

namespace gmafffff.training.hotel.business.Error;

/// <summary>
///     Локализованные сообщения об ошибках
/// </summary>
public class ErrorMessages :
    IErrorMessage<ErrorValidation>,
    IErrorMessage<ErrorBusinessConstraintCheck> {
    public const string Ru = IErrorMessage<ErrorBusinessConstraintCheck>.Ru;
    public const string En = IErrorMessage<ErrorBusinessConstraintCheck>.En;

    // Словарь, где каждому языку соответствует словарь [код ошибки] = "Локализованное сообщение"
    Dictionary<string, Dictionary<ErrorBusinessConstraintCheck, string>>
        IErrorMessage<ErrorBusinessConstraintCheck>.Messages { get; set; } = new() {
        [Ru] = new Dictionary<ErrorBusinessConstraintCheck, string> {
            [ErrorBusinessConstraintCheck.CheckRoomNumberRepeat] =
                "Номер помещения в блоке/корпусе не должен повторяться",
            [ErrorBusinessConstraintCheck.CheckRoomBusy] = "Невозможно выполнить операцию, т.к. номер занят",
            [ErrorBusinessConstraintCheck.CheckPersonLivesInHotel]
                = "Невозможно выполнить операцию, т.к. человек проживает в гостинице",
            [ErrorBusinessConstraintCheck.CheckNumberVisitorsExceedCapacityRoom]
                = "Число посетителей превышает вместимость номера"
        },
        [En] = new Dictionary<ErrorBusinessConstraintCheck, string> {
            [ErrorBusinessConstraintCheck.CheckRoomNumberRepeat] =
                "The room number in the block/case should not be repeated",
            [ErrorBusinessConstraintCheck.CheckRoomBusy]
                = "It is impossible to perform the operation, because the room is busy",
            [ErrorBusinessConstraintCheck.CheckPersonLivesInHotel]
                = "It is impossible to perform the operation, because the person lives in the hotel",
            [ErrorBusinessConstraintCheck.CheckNumberVisitorsExceedCapacityRoom]
                = "The number of visitors exceeds the capacity of the number"
        }
    };

    // Словарь, где каждому языку соответствует словарь [код ошибки] = "Локализованное сообщение"
    Dictionary<string, Dictionary<ErrorValidation, string>>
        IErrorMessage<ErrorValidation>.Messages { get; set; } = new() {
        [Ru] = new Dictionary<ErrorValidation, string> {
            [ErrorValidation.ValidationNotVisitors] = "Не указаны жильцы",
            [ErrorValidation.ValidationDepartureDatePlanned] = "Планируемая дата выезда не может быть в прошлом"
        },
        [En] = new Dictionary<ErrorValidation, string> {
            [ErrorValidation.ValidationNotVisitors] = "Residents are not indicated",
            [ErrorValidation.ValidationDepartureDatePlanned] = "The planned departure date cannot be in the past"
        }
    };
}