using gmafffff.starterKit.AppError;

namespace gmafffff.starterKit.tests.Validation.Fixtures;

public class ValidationErrorCodeMessages : IErrorMessage<ValidationErrorCode> {
    public Dictionary<string, Dictionary<ValidationErrorCode, string>> Messages { get; set; } =
        new() {
            [IErrorMessage<ValidationErrorCode>.Ru] = new Dictionary<ValidationErrorCode, string> {
                [ValidationErrorCode.NameEmpty] = "Не задано имя",
                [ValidationErrorCode.AgeLessThan18] = "Не достиг возраста 18 лет",
                [ValidationErrorCode.ValidationRoomNumberNo] = "Помещению необходим уникальный номер",
                [ValidationErrorCode.ValidationRoomCapacityNo] = "Необходимо указать вместимость номера"
            },
            [IErrorMessage<ValidationErrorCode>.En] = new Dictionary<ValidationErrorCode, string> {
                [ValidationErrorCode.NameEmpty] = "Name is empty",
                [ValidationErrorCode.AgeLessThan18] = "Age Less Than 18",
                [ValidationErrorCode.ValidationRoomNumberNo] = "The room needs a unique number",
                [ValidationErrorCode.ValidationRoomCapacityNo] =
                    "It is necessary to specify the capacity of the number"
            }
        };
}