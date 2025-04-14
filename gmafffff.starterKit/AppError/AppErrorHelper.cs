using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace gmafffff.starterKit.AppError;

/// <summary>
///     Вспомогательный класс для создания ошибок приложения
/// </summary>
/// ///
/// <remarks>
///     Ошибки бывают:
///     <list type="bullet">
///         <item>Exceptional — неожиданные, например, «OutOfMemoryException»</item>
///         <item>Expected — ожидаемые, например «Пользователь не найден»</item>
///         <item>ManyErrors — много ошибок (возможно ноль)</item>
///     </list>
/// </remarks>
public class AppErrorHelper {
    /// <summary>
    ///     Разделитель, используемый в <see cref="ErrorCode2String{TErrorCode}(TErrorCode)" />
    ///     и в <see cref="String2ErrorCode(string)" />
    /// </summary>
    private const string ErrorStringCodeSeparator = " // ";

    static AppErrorHelper() {
        ReScan();
    }

    /// <summary>
    ///     Хранит локализованные сообщения об ошибках для каждого перечисления кодов ошибок
    /// </summary>
    public static IReadOnlyDictionary<Type, Dictionary<string, Dictionary<Enum, string>>>
        Messages { get; private set; } = null!;

    /// <summary>
    ///     Сканирует сборки в поисках сообщений об ошибках
    /// </summary>
    /// <param name="assemblies">
    ///     Список сборок, где осуществляется поиск локализованных сообщений об ошибках.
    ///     Если null, то поиск по всем сборкам <see cref="AppDomain.CurrentDomain" />
    /// </param>
    public static void ReScan(IEnumerable<Assembly>? assemblies = null) {
        //Очистка ранее сохранённых сообщений

        var assembliesToScan = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();
        var messagesHolders = assembliesToScan.SelectMany(assembly => assembly
                .GetTypes()
                .Where(type => type.GetInterfaces().Any(IsAppropriateInterface) &&
                               type.IsClass &&
                               type.GetConstructor(Type.EmptyTypes) != null))
            .ToArray();

        var errorCodesAndMessagesHolders = messagesHolders
            .SelectMany(holderType =>
                holderType.GetInterfaces()
                    .Where(IsAppropriateInterface)
                    .Select(@interface => (TErrorCode: @interface
                        .GenericTypeArguments[0], holderType, @interface)));

        var messages = errorCodesAndMessagesHolders
            .Select(kv => {
                var holder = Activator.CreateInstance(kv.holderType);
                // Так как интерфейс IErrorMessage<> может быть реализован явно, 
                // то напрямую вызывать GetProperty нельзя:
                // var prop = kv.holderType.GetProperty(nameof(IErrorMessage<Enum>.Messages));
                // var value = (IDictionary)prop.GetValue(holder)!;
                // Приходится использовать InterfaceMapping:
                var mapToInterface = kv.holderType.GetInterfaceMap(kv.@interface);
                var get_prop_method = mapToInterface.InterfaceMethods
                    .Single(m => m.Name == $"get_{nameof(IErrorMessage<Enum>.Messages)}");
                var value = (IDictionary)get_prop_method.Invoke(holder, null)!;

                var outerKeys = value.Keys.Cast<string>();
                var outerValues = value.Values.Cast<IDictionary>()
                    .Select(inner => {
                        var innerKeys = inner.Keys.Cast<Enum>();
                        var innerValues = inner.Values.Cast<string>();
                        return innerKeys.Zip(innerValues).ToDictionary();
                    });

                var outerDict = outerKeys.Zip(outerValues).ToDictionary();

                return (kv.TErrorCode, messages: outerDict);
            })
            .ToDictionary();

        Messages = messages.AsReadOnly();

        bool IsAppropriateInterface(Type @interface) {
            return @interface is { IsInterface: true, IsGenericType: true } &&
                   @interface.GetGenericTypeDefinition() == typeof(IErrorMessage<>);
        }
    }

    /// <summary>
    ///     Возвращает словарь ошибок для конкретного Enum типа «TErrorCode»
    /// </summary>
    /// <typeparam name="TErrorCode"></typeparam>
    /// <returns></returns>
    public static Dictionary<string, Dictionary<TErrorCode, string>> GetMessages<TErrorCode>()
        where TErrorCode : struct, Enum {
        var errorCodeType = typeof(TErrorCode);

        var isFind = Messages.TryGetValue(errorCodeType, out var messages);

        if (!isFind) {
            var errorMessage =
                $"""
                 Не найден класс, реализующий {typeof(IErrorMessage<>)} с кодом ошибки типа {errorCodeType},
                 имеющий конструктор без параметров
                 """;
            Debug.WriteLine(errorMessage);
            return [];
        }

        // Некрасивое приведение типа Enum к TErrorCode
        return messages!.Select(outer =>
                (outer.Key, outer.Value
                    .Select(inner => ((TErrorCode)inner.Key, inner.Value))
                    .ToDictionary()))
            .ToDictionary();
    }

    /// <summary>
    ///     Возвращает словарь ошибок для конкретного Enum типа «TErrorCode»
    /// </summary>
    /// <typeparam name="TErrorCode"></typeparam>
    /// <returns></returns>
    public static Dictionary<string, Dictionary<TErrorCode, string>> GetMessages<TErrorCode>(TErrorCode sample)
        where TErrorCode : Enum {
        var errorCodeType = sample.GetType();

        var isFind = Messages.TryGetValue(errorCodeType, out var messages);

        if (!isFind) {
            var errorMessage =
                $"""
                 Не найден класс, реализующий {typeof(IErrorMessage<>)} с кодом ошибки типа {errorCodeType},
                 имеющий конструктор без параметров
                 """;
            Debug.WriteLine(errorMessage);
            return [];
        }

        // Некрасивое приведение типа Enum к TErrorCode
        return messages!.Select(outer =>
                (outer.Key, outer.Value
                    .Select(inner => ((TErrorCode)inner.Key, inner.Value))
                    .ToDictionary()))
            .ToDictionary();
    }

    /// <summary>
    ///     Возвращает локализованное сообщение об ошибке, зарегистрированное в классе производном от
    ///     <see cref="IErrorMessage{TErrorCode}" />
    /// </summary>
    /// <param name="code">Код ошибки</param>
    /// <param name="langName">
    ///     Необязательное название языка.
    ///     По умолчанию будет использоваться <see cref="CultureInfo.CurrentUICulture.Parent.EnglishName" />
    /// </param>
    /// <remarks>
    ///     Если для выбранного языка (по умолчанию <see cref="CultureInfo.CurrentUICulture.Parent.EnglishName" />)
    ///     нет сообщения, то будет использован русский язык.
    ///     Если сообщения нет и на русском, то код ошибки будет переведён в строку
    /// </remarks>
    /// <typeparam name="TErrorCode">Enum значение кода ошибки</typeparam>
    /// <returns></returns>
    public static string GetMessage<TErrorCode>(TErrorCode code, string? langName = null)
        where TErrorCode : Enum {
        var errorMessages = (IDictionary<string, Dictionary<TErrorCode, string>>)GetMessages(code);

        return (
                from lang in Optional(langName) | CultureInfo.CurrentUICulture.Parent.EnglishName
                // Извлекаем сообщения
                from messages in
                    // … локализованные
                    errorMessages.TryGetValue(lang) |
                    // … или на русском языке
                    errorMessages.TryGetValue(IErrorMessage<Enum>.Ru)
                // Приведение типов. Увы
                let messagesLocalized = (IDictionary<TErrorCode, string>)messages
                // Извлекаем сообщение
                from message in
                    // … локализованное
                    messagesLocalized.TryGetValue(code) |
                    // … или на русском языке
                    (
                        // Внутренний запрос:
                        // извлекаем сообщения на русском языке
                        from messages in errorMessages.TryGetValue(IErrorMessage<Enum>.Ru)
                        // Приведение типов. Увы
                        let messagesRu = (IDictionary<TErrorCode, string>)messages
                        // Извлекаем сообщение на русском языке
                        from messageRu in messagesRu.TryGetValue(code)
                        select messageRu)
                select message)
            // Если нигде нет сообщений, то возвращаем код ошибки текстом
            .IfNone(Enum.GetName(code.GetType(), code));
    }

    /// <summary>
    ///     Создает строковое представление кода ошибки
    /// </summary>
    public static string ErrorCode2String<TErrorCode>(TErrorCode code)
        where TErrorCode : Enum {
        return $"{code.GetType().AssemblyQualifiedName}{ErrorStringCodeSeparator}{Enum.GetName(code.GetType(), code)}";
    }

    /// <summary>
    ///     Преобразует строковое представление кода ошибки в конкретный Enum
    /// </summary>
    public static Enum? String2ErrorCode(string codeString) {
        return codeString.Split(ErrorStringCodeSeparator) is [var typeName and not null, var value and not null]
               && Type.GetType(typeName) is { } type
               && Enum.TryParse(type, value, out var result)
            ? (Enum?)result
            : null;
    }

    /// <summary>
    ///     Создаёт «ожидаемую» ошибку
    /// </summary>
    /// <remarks>
    ///     Ошибки бывают:
    ///     <list type="bullet">
    ///         <item>Exceptional — неожиданные, например, «OutOfMemoryException»</item>
    ///         <item>Expected — ожидаемые, например «Пользователь не найден»</item>
    ///         <item>ManyErrors — много ошибок (возможно ноль)</item>
    ///     </list>
    /// </remarks>
    /// <param name="code">Код ошибки</param>
    /// <param name="exception">Опционально ожидаемое исключение</param>
    /// <returns></returns>
    public static Error NewError<TErrorCode>(TErrorCode code, Exception? exception = null)
        where TErrorCode : Enum {
        var message = GetMessage(code);
        // Прямое приведение enum к целочисленному типу (например, (int)code) не работает с универсальным типом
        var codeInteger = (int)Convert.ChangeType(code, code.GetTypeCode());

        var error = exception is null
            ? Error.New(codeInteger, message)
            : Error.New(codeInteger, message, ErrorException.New(exception));

        return error;
    }

    /// <summary>
    ///     Создаёт комплексную «ожидаемую» ошибку, состоящую из нескольких ошибок
    /// </summary>
    /// <remarks>
    ///     Ошибки бывают:
    ///     <list type="bullet">
    ///         <item>Exceptional — неожиданные, например, «OutOfMemoryException»</item>
    ///         <item>Expected — ожидаемые, например «Пользователь не найден»</item>
    ///         <item>ManyErrors — много ошибок (возможно ноль)</item>
    ///     </list>
    /// </remarks>
    /// <param name="codes">Коды объединяемых ошибок</param>
    public static Error NewError<TErrorCode>(IEnumerable<TErrorCode> codes)
        where TErrorCode : Enum {
        return Error.Many(codes.Select(code => NewError(code)).ToArray());
    }

    /// <summary>
    ///     Создаёт «неожиданную» ошибку из исключения
    /// </summary>
    /// <remarks>
    ///     Ошибки бывают:
    ///     <list type="bullet">
    ///         <item>Exceptional — неожиданные, например, «OutOfMemoryException»</item>
    ///         <item>Expected — ожидаемые, например «Пользователь не найден»</item>
    ///         <item>ManyErrors — много ошибок (возможно ноль)</item>
    ///     </list>
    /// </remarks>
    /// <param name="exception">Неожиданное исключение</param>
    /// <returns></returns>
    public static Error NewError(Exception exception) {
        return Error.New(exception);
    }
}