using System.ComponentModel.DataAnnotations;

namespace gmafffff.training.hotel.application.PersonManagement.Dto;

public record PersonFindDto(
    [Display(AutoGenerateField = false)] Guid PersonId,
    [Display(Name = "ФИО")] string FullName
);