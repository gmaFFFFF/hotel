namespace gmafffff.training.hotel.domain.Model;

/// <summary>
///     ФИО
/// </summary>
/// <param name="SurName"></param>
/// <param name="FirstName"></param>
/// <param name="Patronymic"></param>
public record PersonFullName(string SurName, string FirstName, string? Patronymic);