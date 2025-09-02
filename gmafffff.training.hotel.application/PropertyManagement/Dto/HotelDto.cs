using System.ComponentModel.DataAnnotations;

namespace gmafffff.training.hotel.application.PropertyManagement.Dto;

/// <summary>
///     Номерной фонд
/// </summary>
/// <param name="BlockId">Идентификатор блока(корпуса/здания)</param>
/// <param name="Rooms">Номерной фонд</param>
public record HotelDto(
    [Display(AutoGenerateField = false)] int BlockId,
    [Display(Name = "Номера")] ICollection<RoomDto> Rooms);