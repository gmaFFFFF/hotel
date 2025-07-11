using gmafffff.training.hotel.domain.PropertyManagement.Dto;

namespace gmafffff.training.hotel.business.PropertyManagement.Commands;

public record AddRoomCommand(RoomAddDto Room) : BusinessCommand;