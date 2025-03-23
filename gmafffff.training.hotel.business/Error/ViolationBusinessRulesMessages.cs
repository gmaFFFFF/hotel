using gmafffff.starterKit.AppError;

namespace gmafffff.training.hotel.business.Error;

/// <summary>
///     Локализованные сообщения об ошибках
/// </summary>
public class ViolationBusinessRulesMessages : IErrorMessage<ViolationBusinessRules> {
    public const string Ru = IErrorMessage<ViolationBusinessRules>.Ru;
    public const string En = IErrorMessage<ViolationBusinessRules>.En;

    // Словарь, где каждому языку соответствует словарь [код ошибки] = "Локализованное сообщение"
    public Dictionary<string, Dictionary<ViolationBusinessRules, string>>
        Messages { get; set; } = new() {
        [Ru] = new Dictionary<ViolationBusinessRules, string> {
            [ViolationBusinessRules.RuleRoomNumberRepeat] = "Номер помещения в блоке/корпусе не должен повторяться",
            [ViolationBusinessRules.RuleRoomBusy] = "Невозможно выполнить операцию, т.к. номер занят",
            [ViolationBusinessRules.PersonLivesInHotel] = "Невозможно выполнить операцию, " +
                                                          "т.к. человек проживает в гостинице"
        },
        [En] = new Dictionary<ViolationBusinessRules, string> {
            [ViolationBusinessRules.RuleRoomNumberRepeat] = "The room number in the block/case should not be repeated",
            [ViolationBusinessRules.RuleRoomBusy] = "It is impossible to perform the operation, " +
                                                    "because the room is busy",
            [ViolationBusinessRules.PersonLivesInHotel] = "It is impossible to perform the operation, " +
                                                          "because the person lives in the hotel"
        }
    };
}