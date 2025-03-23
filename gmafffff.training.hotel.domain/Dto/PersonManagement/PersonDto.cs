using System.ComponentModel.DataAnnotations;

namespace gmafffff.training.hotel.domain.Dto.PersonManagement;

public record PersonDto(
    [Display(Name = "Ид")] Guid PersonId,
    [Display(Name = "Фамилия")] string SurName,
    [Display(Name = "Имя")] string FirstName,
    [Display(Name = "Отчество")] string? Patronymic
);