using gmafffff.training.hotel.domain.Dto.PropertyManagement;

namespace gmafffff.training.hotel.business.PropertyManagement.Commands;

public record UpdateRoomCommand(int Id, RoomUpdateDto RoomUpdate) : BusinessCommand;