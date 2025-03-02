namespace gmafffff.starterKit.Messaging;

/// <summary>
///     Интерфейс сообщений, используемый для организации общения между UI/Api и системой
/// </summary>
/// <typeparam name="T">Тип идентификатора сообщения</typeparam>
public interface IMessage<T> where T : struct {
    T MessageId { get; init; }
}