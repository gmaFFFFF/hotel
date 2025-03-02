namespace gmafffff.starterKit.AppError;

/// <summary>
///     Локализованные сообщения об ошибках
/// </summary>
public interface IErrorMessage<TErrorCode>
    where TErrorCode : Enum {
    public const string Ru = "Russian";
    public const string En = "English";

    // Словарь, где каждому [названию языка] соответствует словарь [код ошибки] = "Локализованное сообщение"
    public Dictionary<string, Dictionary<TErrorCode, string>> Messages { get; set; }
}