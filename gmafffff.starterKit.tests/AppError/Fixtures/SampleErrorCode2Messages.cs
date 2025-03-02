using gmafffff.starterKit.AppError;

namespace gmafffff.starterKit.tests.AppError.Fixtures;

public class SampleErrorCode2Messages : IErrorMessage<SampleErrorCode2> {
    public Dictionary<string, Dictionary<SampleErrorCode2, string>> Messages { get; set; } =
        new() {
            [IErrorMessage<SampleErrorCode2>.Ru] = new Dictionary<SampleErrorCode2, string> {
                [SampleErrorCode2.BiLang] = "Операция отменена",
                [SampleErrorCode2.OnlyRus] = "Непредвиденное завершение работы"
            },
            [IErrorMessage<SampleErrorCode2>.En] = new Dictionary<SampleErrorCode2, string> {
                [SampleErrorCode2.BiLang] = "Operation canceled"
            }
        };
}