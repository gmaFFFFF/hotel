namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     История посещений гостиницы
/// </summary>
/// <param name="Count">Количество посещений</param>
/// <param name="Duration">Общая длительность</param>
public record VisitorHistory (int Count = 0, int Duration = 0);