using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Validot;
using Validot.Results;

namespace gmafffff.starterKit.Validation;

/// <summary>
///     Позволяет производным классам выполнять самопроверку с помощью зарегистрированных сервисов
///     <see cref="IValidator{T}" />
/// </summary>
public interface IValidatableObjectHelperValidot : IValidatableObject {
    private static string ExpectedTranslationName => CultureInfo.CurrentUICulture.Parent.EnglishName;

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) {
        var validatorType = typeof(IValidator<>).MakeGenericType(GetType());
        var validator = validationContext.GetService(validatorType);
        if (validator is null) return [];

        var result = validator.GetType()
            .GetMethod(nameof(IValidator<object>.Validate))!
            .Invoke(validator, [this, false]) as IValidationResult;

        return result!.AnyErrors
            ? ConvertToValidationResult(GetLocalizedMessageMap(result))
            : [];
    }

    /// <summary>
    ///     Преобразует <see cref="IValidationResult" /> в словарь [поле/свойство] = "описание ошибки".
    ///     При возможности возвращает локализованную версию описания ошибок.
    /// </summary>
    private static IReadOnlyDictionary<string, IReadOnlyList<string>> GetLocalizedMessageMap(IValidationResult result) {
        return result.TranslationNames.Contains(ExpectedTranslationName)
            ? result.GetTranslatedMessageMap(ExpectedTranslationName)
            : result.MessageMap;
    }

    /// <summary>
    ///     Преобразует словарь ошибок ([поле/свойство] = "описание ошибки") в <see cref="ValidationResult" />
    /// </summary>
    private static IEnumerable<ValidationResult> ConvertToValidationResult(
        IReadOnlyDictionary<string, IReadOnlyList<string>> result) {
        return from property in result.Keys
            from error in result[property]
            select string.IsNullOrWhiteSpace(property)
                ? new ValidationResult(error)
                : new ValidationResult(error, [property]);
    }
}