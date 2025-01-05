namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     Описание тарифа
/// </summary>
/// <param name="Type">Класс номера</param>
/// <param name="Value">Цена за ночь</param>
public record TariffDetails(RoomType Type, decimal Value);