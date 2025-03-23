using gmafffff.starterKit.Validation;
using gmafffff.training.hotel.domain.Dto.PropertyManagement;
using gmafffff.training.hotel.domain.Error;
using Validot;

namespace gmafffff.training.hotel.domain.Validation;

public class RoomSpec : LocalizedSpecification,
    ISpecificationHolder<RoomDto>,
    ISpecificationHolder<RoomAddDto>,
    ISpecificationHolder<RoomUpdateDto>,
    ISpecificationHolder<RoomDetails> {
    private readonly Specification<RoomAddDto> _roomAddDtoSpec;
    private readonly Specification<RoomDetails> _roomDetailsSpec;
    private readonly Specification<RoomDto> _roomDtoSpec;
    private readonly Specification<RoomUpdateDto> _roomUpdateDtoSpec;

    public RoomSpec() {
        Specification<string> numberSpec = s => s
            .NotEmpty()
            .WithErrorCode(ErrorCode.ValidationRoomNumberNo)
            .NotWhiteSpace()
            .WithErrorCode(ErrorCode.ValidationRoomNumberNo);


        Specification<byte> capacitySpec = s => s
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(ErrorCode.ValidationRoomCapacityNo);


        _roomDetailsSpec = s => s
            .Member(memberSelector: m => m.Number, numberSpec)
            .Member(memberSelector: m => m.Capacity, capacitySpec);

        _roomDtoSpec = s => s
            .Member(memberSelector: m => m.Number, numberSpec)
            .Member(memberSelector: m => m.Capacity, capacitySpec);

        _roomAddDtoSpec = s => s
            .Member(memberSelector: m => m.Number, numberSpec)
            .Member(memberSelector: m => m.Capacity, capacitySpec);

        _roomUpdateDtoSpec = s => s
            .Member(memberSelector: m => m.Number, numberSpec)
            .Member(memberSelector: m => m.Capacity, capacitySpec);
    }

    Specification<RoomAddDto> ISpecificationHolder<RoomAddDto>.Specification => _roomAddDtoSpec;
    Specification<RoomDetails> ISpecificationHolder<RoomDetails>.Specification => _roomDetailsSpec;
    Specification<RoomDto> ISpecificationHolder<RoomDto>.Specification => _roomDtoSpec;
    Specification<RoomUpdateDto> ISpecificationHolder<RoomUpdateDto>.Specification => _roomUpdateDtoSpec;
}