using System.Globalization;
using gmafffff.starterKit.AppError;
using gmafffff.starterKit.tests.AppError.Fixtures;

namespace gmafffff.starterKit.tests.AppError;

[TestSubject(typeof(AppErrorHelper))]
public class AppErrorHelperTests {
    public static TheoryData<Enum, string, Type> RusData {
        get {
            var messages = new SampleErrorCode1Messages().Messages[IErrorMessage<Enum>.Ru];
            var messages2 = new SampleErrorCode2Messages().Messages[IErrorMessage<Enum>.Ru];
            var codeType1 = typeof(SampleErrorCode1);
            var codeType2 = typeof(SampleErrorCode2);
            return new TheoryData<Enum, string, Type> {
                { SampleErrorCode1.None, Enum.GetName(SampleErrorCode1.None), codeType1 },
                { SampleErrorCode1.BiLang, messages[SampleErrorCode1.BiLang], codeType1 },
                { SampleErrorCode1.OnlyRus, messages[SampleErrorCode1.OnlyRus], codeType1 },
                { SampleErrorCode1.WithoutMessage, Enum.GetName(SampleErrorCode1.WithoutMessage), codeType1 },
                { SampleErrorCode2.None, Enum.GetName(SampleErrorCode2.None), codeType2 },
                { SampleErrorCode2.BiLang, messages2[SampleErrorCode2.BiLang], codeType2 },
                { SampleErrorCode2.OnlyRus, messages2[SampleErrorCode2.OnlyRus], codeType2 },
                { SampleErrorCode2.WithoutMessage, Enum.GetName(SampleErrorCode2.WithoutMessage), codeType2 }
            };
        }
    }

    public static TheoryData<Enum, string, Type> EnData {
        get {
            var messages = new SampleErrorCode1Messages().Messages[IErrorMessage<Enum>.En];
            var messagesRus = new SampleErrorCode1Messages().Messages[IErrorMessage<Enum>.Ru];
            var messages2 = new SampleErrorCode2Messages().Messages[IErrorMessage<Enum>.En];
            var messages2Rus = new SampleErrorCode2Messages().Messages[IErrorMessage<Enum>.Ru];

            var codeType1 = typeof(SampleErrorCode1);
            var codeType2 = typeof(SampleErrorCode2);
            return new TheoryData<Enum, string, Type> {
                { SampleErrorCode1.None, Enum.GetName(SampleErrorCode1.None), codeType1 },
                { SampleErrorCode1.BiLang, messages[SampleErrorCode1.BiLang], codeType1 },
                { SampleErrorCode1.OnlyRus, messagesRus[SampleErrorCode1.OnlyRus], codeType1 },
                { SampleErrorCode1.WithoutMessage, Enum.GetName(SampleErrorCode1.WithoutMessage), codeType1 },
                { SampleErrorCode2.None, Enum.GetName(SampleErrorCode2.None), codeType2 },
                { SampleErrorCode2.BiLang, messages2[SampleErrorCode2.BiLang], codeType2 },
                { SampleErrorCode2.OnlyRus, messages2Rus[SampleErrorCode2.OnlyRus], codeType2 },
                { SampleErrorCode2.WithoutMessage, Enum.GetName(SampleErrorCode2.WithoutMessage), codeType2 }
            };
        }
    }

    /// <summary>
    ///     Может извлекать русские сообщения для ошибок
    /// </summary>
    [Theory]
    [MemberData(nameof(RusData))]
    public void CanExtractMessageForErrorRus(object code, string exceptedMessage, Type codeType) {
        // Arrange
        var currentCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo("he-IL");

        // Act
        var message =
            codeType == typeof(SampleErrorCode1)
                ? AppErrorHelper.GetMessage((SampleErrorCode1)code, IErrorMessage<Enum>.Ru)
                : AppErrorHelper.GetMessage((SampleErrorCode2)code, IErrorMessage<Enum>.Ru);

        // Assert
        message.Should().Be(exceptedMessage);

        CultureInfo.CurrentUICulture = currentCulture;
    }

    /// <summary>
    ///     Может извлекать английские сообщения для ошибок
    /// </summary>
    [Theory]
    [MemberData(nameof(EnData))]
    public void CanExtractMessageForErrorEn(object code, string exceptedMessage, Type codeType) {
        // Arrange
        var currentCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo("he-IL");

        // Act
        var message = codeType == typeof(SampleErrorCode1)
            ? AppErrorHelper.GetMessage((SampleErrorCode1)code, IErrorMessage<Enum>.En)
            : AppErrorHelper.GetMessage((SampleErrorCode2)code, IErrorMessage<Enum>.En);

        // Assert
        message.Should().Be(exceptedMessage);
        CultureInfo.CurrentUICulture = currentCulture;
    }

    /// <summary>
    ///     Может извлекать сообщения для текущей культуры (русской)
    /// </summary>
    [Theory]
    [MemberData(nameof(RusData))]
    public void CanExtractMessageForErrorCurrentRuCulture(object code, string exceptedMessage, Type codeType) {
        // Arrange
        var currentCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo("ru-Ru");

        // Act
        var message = codeType == typeof(SampleErrorCode1)
            ? AppErrorHelper.GetMessage((SampleErrorCode1)code)
            : AppErrorHelper.GetMessage((SampleErrorCode2)code);

        // Assert
        message.Should().Be(exceptedMessage);

        CultureInfo.CurrentUICulture = currentCulture;
    }

    /// <summary>
    ///     Может извлекать сообщения для текущей культуры (английской)
    /// </summary>
    [Theory]
    [MemberData(nameof(EnData))]
    public void CanExtractMessageForErrorCurrentEnCulture(object code, string exceptedMessage, Type codeType) {
        // Arrange
        var currentCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo("en-En");

        // Act
        var message = codeType == typeof(SampleErrorCode1)
            ? AppErrorHelper.GetMessage((SampleErrorCode1)code)
            : AppErrorHelper.GetMessage((SampleErrorCode2)code);

        // Assert
        message.Should().Be(exceptedMessage);

        CultureInfo.CurrentUICulture = currentCulture;
    }

    /// <summary>
    ///     Может конвертировать код ошибки в строковое представление и обратно
    /// </summary>
    [Fact]
    public void CanConvertErrorCodeIntoStringAndBack() {
        // Arrange
        var code = SampleErrorCode1.OnlyRus;

        // Act
        var codeString = AppErrorHelper.ErrorCode2String(code);
        var back = AppErrorHelper.String2ErrorCode(codeString);

        // Assert
        back.Should().Be(code);
    }
}