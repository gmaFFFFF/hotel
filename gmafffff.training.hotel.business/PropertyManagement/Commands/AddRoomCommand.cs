using gmafffff.training.hotel.domain.Dto.PropertyManagement;

namespace gmafffff.training.hotel.business.PropertyManagement.Commands;

public record AddRoomCommand(RoomAddDto Room) : BusinessCommand;