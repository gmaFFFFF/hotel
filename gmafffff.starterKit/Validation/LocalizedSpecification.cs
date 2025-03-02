using gmafffff.starterKit.AppError;
using Validot;
using Validot.Factory;
using Validot.Settings;

namespace gmafffff.starterKit.Validation;

/// <summary>
///     Базовый класс для создания локализованных проверочных спецификаций
/// </summary>
public abstract class LocalizedSpecification : ISettingsHolder {
    public virtual Func<ValidatorSettings, ValidatorSettings> Settings => s =>
        // Локализация сообщений, поставляемых с библиотекой Validot
        s.WithRussianTranslation()
            // Локализация сообщений, используемых приложением
            .WithTranslation(
                new Dictionary<string, IReadOnlyDictionary<string, string>> {
                    [IErrorMessage<Enum>.Ru] =
                        (from allError in AppErrorHelper.Messages.Values
                            from code in allError[IErrorMessage<Enum>.Ru].Keys
                            select (AppErrorHelper.ErrorCode2String(code), allError[IErrorMessage<Enum>.Ru][code]))
                        .ToDictionary(),
                    [IErrorMessage<Enum>.En] =
                        (from allError in AppErrorHelper.Messages.Values
                            from code in allError[IErrorMessage<Enum>.En].Keys
                            select (AppErrorHelper.ErrorCode2String(code), allError[IErrorMessage<Enum>.En][code]))
                        .ToDictionary()
                });
}