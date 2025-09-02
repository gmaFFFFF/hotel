using System.ComponentModel.DataAnnotations;
using gmafffff.training.hotel.domain.PropertyManagement.Models;
using Mapster;

namespace gmafffff.training.hotel.business.tests.Fixtures;

/// <summary>
///     Номер гостиницы
/// </summary>
public record RoomDto(
    [Display(AutoGenerateField = false)] int RoomId,
    [Display(Name = "Номер", ShortName = "№")]
    string Number,
    [Display(Name = "Категория")] RoomType Type,
    [Display(Name = "Вместимость")] byte Capacity) {
    public static readonly TypeAdapterConfig MapperConfig = new();

    static RoomDto() {
        MapperConfig.NewConfig<Room<Guid>, RoomDto>()
            .TwoWays()
            .Map(member: d => d.RoomId, source: s => s.Id)
            .Map(member: d => d, source: s => s.RoomDetails);
    }
}