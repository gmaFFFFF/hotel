using System.ComponentModel.DataAnnotations;

namespace gmafffff.training.hotel.application.PersonManagement.Dto;

public record PersonDto(
    [Display(AutoGenerateField = false)] Guid PersonId,
    [Display(Name = "Фамилия")] string SurName,
    [Display(Name = "Имя")] string FirstName,
    [Display(Name = "Отчество")] string? Patronymic
);