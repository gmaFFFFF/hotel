using System.ComponentModel.DataAnnotations;
using gmafffff.training.hotel.domain.PropertyManagement.Models;

namespace gmafffff.training.hotel.application.PropertyManagement.Dto;

/// <summary>
///     Номер гостиницы
/// </summary>
public record RoomDto(
    [Display(AutoGenerateField = false)] int RoomId,
    [Display(Name = "Номер", ShortName = "№")]
    string Number,
    [Display(Name = "Категория")] RoomType Type,
    [Display(Name = "Вместимость")] byte Capacity);