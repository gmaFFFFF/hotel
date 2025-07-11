using System.ComponentModel.DataAnnotations;

namespace gmafffff.training.hotel.domain.PersonManagement.Dto;

public record PersonUpdateDto(
    [Display(Name = "Фамилия")] string SurName,
    [Display(Name = "Имя")] string FirstName,
    [Display(Name = "Отчество")] string? Patronymic)
    : PersonAddDto(SurName, FirstName, Patronymic);