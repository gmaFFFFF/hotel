using System.ComponentModel.DataAnnotations;

namespace gmafffff.training.hotel.domain.PersonManagement.Dto;

public record PersonAddDto(
    [Display(Name = "Фамилия")] string SurName,
    [Display(Name = "Имя")] string FirstName,
    [Display(Name = "Отчество")] string? Patronymic
);