namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     Описание номера
/// </summary>
/// <param name="Number">Номер</param>
/// <param name="Type">Класс номера</param>
/// <param name="Capacity">Вместимость</param>
public record RoomDetails(string Number, RoomType Type = RoomType.Standard, byte Capacity = 2);