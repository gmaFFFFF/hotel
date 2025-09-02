using System.ComponentModel.DataAnnotations;
using gmafffff.training.hotel.domain.PersonManagement.Models;
using Mapster;

namespace gmafffff.training.hotel.business.tests.Fixtures;

public record PersonDto(
    [Display(AutoGenerateField = false)] Guid PersonId,
    [Display(Name = "Фамилия")] string SurName,
    [Display(Name = "Имя")] string FirstName,
    [Display(Name = "Отчество")] string? Patronymic) {
    public static readonly TypeAdapterConfig MapperConfig = new();

    static PersonDto() {
        MapperConfig.NewConfig<Person<Guid>, PersonDto>()
            .TwoWays()
            .Map(member: d => d.PersonId, source: s => s.Id)
            .Map(member: d => d, source: s => s.FullName);
    }
}