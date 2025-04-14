using gmafffff.starterKit.AppError;

namespace gmafffff.training.hotel.business.Error;

/// <summary>
///     Локализованные сообщения об ошибках
/// </summary>
public class ErrorMessages : 
    IErrorMessage<ErrorValidation>, 
    IErrorMessage<ErrorBusinessRules>{
    public const string Ru = IErrorMessage<ErrorBusinessRules>.Ru;
    public const string En = IErrorMessage<ErrorBusinessRules>.En;

    // Словарь, где каждому языку соответствует словарь [код ошибки] = "Локализованное сообщение"
    Dictionary<string, Dictionary<ErrorBusinessRules, string>>
        IErrorMessage<ErrorBusinessRules>.Messages { get; set; } = new() {
        [Ru] = new Dictionary<ErrorBusinessRules, string> {
            [ErrorBusinessRules.RuleRoomNumberRepeat] = "Номер помещения в блоке/корпусе не должен повторяться",
            [ErrorBusinessRules.RuleRoomBusy] = "Невозможно выполнить операцию, т.к. номер занят",
            [ErrorBusinessRules.RulePersonLivesInHotel] 
                = "Невозможно выполнить операцию, т.к. человек проживает в гостинице",
            [ErrorBusinessRules.RuleNumberVisitorsExceedCapacityRoom] 
                = "Число посетителей превышает вместимость номера",                                                          
        },
        [En] = new Dictionary<ErrorBusinessRules, string> {
            [ErrorBusinessRules.RuleRoomNumberRepeat] = "The room number in the block/case should not be repeated",
            [ErrorBusinessRules.RuleRoomBusy] 
                = "It is impossible to perform the operation, because the room is busy",
            [ErrorBusinessRules.RulePersonLivesInHotel]
                = "It is impossible to perform the operation, because the person lives in the hotel",
            [ErrorBusinessRules.RuleNumberVisitorsExceedCapacityRoom] 
                = "The number of visitors exceeds the capacity of the number",                                                          
        }
    };

    // Словарь, где каждому языку соответствует словарь [код ошибки] = "Локализованное сообщение"
    Dictionary<string, Dictionary<ErrorValidation, string>>
        IErrorMessage<ErrorValidation>.Messages { get; set; } = new() {
        [Ru] = new Dictionary<ErrorValidation, string> {
            [ErrorValidation.ValidationNotVisitors] = "Не указаны жильцы",             
            [ErrorValidation.ValidationDepartureDatePlanned] = "Планируемая дата выезда не может быть в прошлом",             
        },
        [En] = new Dictionary<ErrorValidation, string> {
            [ErrorValidation.ValidationNotVisitors] = "Residents are not indicated",                 
            [ErrorValidation.ValidationDepartureDatePlanned] = "The planned departure date cannot be in the past",                 
        }
    };
}