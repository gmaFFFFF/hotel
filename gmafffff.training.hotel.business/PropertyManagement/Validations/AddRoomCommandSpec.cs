using gmafffff.starterKit.Validation;
using gmafffff.training.hotel.business.PropertyManagement.Commands;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Validation;
using Validot;

namespace gmafffff.training.hotel.business.PropertyManagement.Validations;

public class AddRoomCommandSpec : LocalizedSpecification,
    ISpecificationHolder<AddRoomCommand> {
    public AddRoomCommandSpec() {
        // Приведение типа к ISpecificationHolder<RoomAddDto> потребовалось, т.к. интерфейс реализован явно
        var roomAddDtoSpec = ((ISpecificationHolder<RoomAddDto>)new RoomSpec()).Specification;
        Specification = s => s
            .Member(memberSelector: m => m.Room, roomAddDtoSpec);
    }

    public Specification<AddRoomCommand> Specification { get; }
}