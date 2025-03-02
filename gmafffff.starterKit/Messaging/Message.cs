namespace gmafffff.starterKit.Messaging;

/// <summary>
///     Реализация интерфейса <see cref="IMessage{T}" />, в котором идентификатор имеет тип <see cref="Guid" />
/// </summary>
/// <param name="MessageId">Идентификатор сообщения</param>
public abstract record Message(Guid MessageId = default) : IMessage<Guid> {
    public Guid MessageId { get; init; } = MessageId == default ? NextGuid() : MessageId;

    /// <summary>
    ///     Генератор последовательных Guid
    /// </summary>
    /// <remarks>
    ///     Источник:
    ///     <see
    ///         href="https://github.com/nhibernate/nhibernate-core/blob/3087d48640eb64f573c0011e9a9b1567afe6adde/src/NHibernate/Id/GuidCombGenerator.cs" />
    /// </remarks>
    /// <returns></returns>
    private static Guid NextGuid() {
        var guidArray = Guid.NewGuid().ToByteArray();

        // Базовая дата — Войска 1-го Украинского фронта вышли на Государственную границу СССР
        var baseDate = new DateTime(year: 1944, month: 03, day: 26);
        var now = DateTime.Now;

        // Get the days and milliseconds which will be used to build the byte string 
        var days = new TimeSpan(now.Ticks - baseDate.Ticks);
        var msecs = now.TimeOfDay;

        // Convert to a byte array 
        // Note that SQL Server is accurate to 1/300th of a millisecond, so we divide by 3.333333 
        var daysArray = BitConverter.GetBytes(days.Days);
        var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

        // Reverse the bytes to match SQL Servers ordering 
        Array.Reverse(daysArray);
        Array.Reverse(msecsArray);

        // Copy the bytes into the guid 
        Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, length: 2);
        Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, length: 4);

        return new Guid(guidArray);
    }
}