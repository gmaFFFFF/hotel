using System.ComponentModel.DataAnnotations;
using gmafffff.starterKit.Validation;
using gmafffff.training.hotel.domain.Model;

namespace gmafffff.training.hotel.business.PropertyManagement.Dto;

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