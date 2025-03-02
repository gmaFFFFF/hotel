using gmafffff.starterKit.AppError;

namespace gmafffff.starterKit.tests.AppError.Fixtures;

public class SampleErrorCode1Messages : IErrorMessage<SampleErrorCode1> {
    public Dictionary<string, Dictionary<SampleErrorCode1, string>> Messages { get; set; } =
        new() {
            [IErrorMessage<SampleErrorCode1>.Ru] = new Dictionary<SampleErrorCode1, string> {
                [SampleErrorCode1.BiLang] = "Некорректные данные",
                [SampleErrorCode1.OnlyRus] = "Запись не найдена"
            },
            [IErrorMessage<SampleErrorCode1>.En] = new Dictionary<SampleErrorCode1, string> {
                [SampleErrorCode1.BiLang] = "Not correct data"
            }
        };
}