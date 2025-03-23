using System.ComponentModel.DataAnnotations;

namespace gmafffff.training.hotel.domain.Dto.PropertyManagement;

/// <summary>
///     Добавляемый номер гостиницы
/// </summary>
public record RoomAddDto(
    [Display(Name = "Ид здания/корпуса")] int HotelBlockId,
    [Display(Name = "Номер", ShortName = "№")]
    string Number,
    [Display(Name = "Категория")] RoomType Type,
    [Display(Name = "Вместимость")] byte Capacity) : RoomUpdateDto(Number, Type, Capacity);