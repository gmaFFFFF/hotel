using System.Text;
using gmafffff.starterKit.AppError;
using LanguageExt.Common;
using Validot.Results;
using Validot.Specification;

namespace Validot;

public static class ValidotExtensions {
    public const string PrefixAppInnerCode = "appErrorCode:\\";

    /// <summary>
    ///     По заданному коду ошибки <paramref name="error" /> заполняет информацию, передаваемую в методы
    ///     <see cref="WithMessageExtension.WithMessage{T}(IWithMessageIn{T}, string)" /> и
    ///     <see cref="WithExtraCodeExtension.WithExtraCode{T}(IWithExtraCodeIn{T}, string)" />,
    ///     т.е.:
    ///     <list type="number">
    ///         <item>ключ к локализованному сообщению об ошибке (для человека);</item>
    ///         <item>краткий строковый код ошибки (для парсеров).</item>
    ///     </list>
    /// </summary>
    public static IWithExtraCodeOut<T> WithErrorCode<T, TError>(this IRuleOut<T> @this, TError error)
        where TError : Enum {
        return @this
            .WithMessage(AppErrorHelper.ErrorCode2String(error))
            .WithExtraCode($"{typeof(TError).Name}.{Enum.GetName(typeof(TError), error)}")

            // Validot не сохраняет (во всяком случае в общем доступе) ключ к сообщению, 
            // поэтому чтобы иметь возможность извлечь Enum код ошибки
            // сохраняем строковое представление кода ошибки.
            // Validot требует чтобы в коде не встречались пробелы, поэтому кодируем в base64
            // Префикс поможет отыскивать ключ к сообщению
            // Используется в методе ToExceptedError
            .WithExtraCode($"{PrefixAppInnerCode}{Base64Encode(AppErrorHelper.ErrorCode2String(error))}");
    }

    /// <summary>
    ///     Преобразует результат проверки в enum-коды ошибок
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static IEnumerable<Enum> ToErrorCodes(this IValidationResult @this) {
        return @this.Codes
            .Where(code => code.StartsWith(PrefixAppInnerCode))
            .Select(code => code.Remove(startIndex: 0, PrefixAppInnerCode.Length))
            .Select(Base64Decode)
            .Select(AppErrorHelper.String2ErrorCode)
            .OfType<Enum>();
    }

    /// <summary>
    ///     Преобразует результат проверки в ожидаемые ошибки приложения <see cref="LanguageExt.Common.Expected" />
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static Error ToExceptedError(this IValidationResult @this) {
        if (!@this.AnyErrors)
            return Error.Empty;

        var errorCodes = @this.ToErrorCodes().ToArray();

        return errorCodes.Length == 1
            ? AppErrorHelper.NewError(errorCodes.Single())
            : AppErrorHelper.NewError(errorCodes);
    }

    /// <remarks>
    ///     Источник: <see href="https://stackoverflow.com/a/11743162/6803090" />
    /// </remarks>
    private static string Base64Encode(string plainText) {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    /// <remarks>
    ///     Источник: <see href="https://stackoverflow.com/a/11743162/6803090" />
    /// </remarks>
    private static string Base64Decode(string base64EncodedData) {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}