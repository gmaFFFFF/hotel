using System.ComponentModel.DataAnnotations;
using gmafffff.starterKit.Validation;

namespace gmafffff.training.hotel.domain.Dto.PropertyManagement;

/// <summary>
///     Номер гостиницы
/// </summary>
public record RoomDto(
    [Display(Name = "Ид")] int RoomId,
    [Display(Name = "Номер", ShortName = "№")]
    string Number,
    [Display(Name = "Категория")] RoomType Type,
    [Display(Name = "Вместимость")] byte Capacity)
    : IValidatableObjectHelperValidot;