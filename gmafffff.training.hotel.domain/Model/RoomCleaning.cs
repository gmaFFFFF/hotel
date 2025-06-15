using gmafffff.starterKit.Domain;

namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     Уборка номера
/// </summary>
public class RoomCleaning : Entity<int> {
    /// <summary>
    ///     Гостиничный номер, который необходимо убрать
    /// </summary>
    public Room<Guid> Room { get; set; }

    /// <summary>
    ///     Момент постановки в очередь на уборку
    /// </summary>
    public DateTime PutInLine { get; set; }

    /// <summary>
    ///     Момент завершения уборки
    /// </summary>
    public DateTime? CleaningCompleted { get; set; }

    /// <summary>
    ///     Убран ли номер
    /// </summary>
    public bool IsClean { get; }
}