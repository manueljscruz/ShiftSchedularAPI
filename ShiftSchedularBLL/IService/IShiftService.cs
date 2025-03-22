
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularBLL.IService
{
    public interface IShiftService
    {
        Task<BaseResponse<ShiftDTO>> AddEntityShift(AddShiftDTO shiftDTO);
        Task<BaseResponse<ShiftBreakDTO>> AddEntityShiftBreak(AddShiftBreakDTO shiftBreakDTO);
        Task<BaseResponse<bool>> DeleteEntityShift(string entityId, string shiftId);
        Task<BaseResponse<bool>> DeleteEntityShiftBreak(string shiftBreakId);
        Task<ShiftViewModel> GetEntityShiftsViewModel(BaseViewModelRequest shiftViewModelRequestDTO);
        Task<ShiftDTO> GetShiftById(string shiftId, string lcode);
        Task<IEnumerable<ShiftDTO>> GetEntityShifts(Guid entityId);
        Task<BaseResponse<bool>> UpdateEntityShift(ShiftDTO shift);
        Task<BaseResponse<bool>> UpdateEntityShiftBreak(ShiftBreakDTO shiftBreak);
        Task<List<ShiftDTO>> GetSpecificShifts(List<string> shiftIdentifiers);
        Task<List<EntityShiftRotationDTO>> GetEntityShiftRotations(Guid entityId);
        Task<BaseResponse<EntityShiftRotationDTO>> AddShiftRotation(AddShiftRotationDTO rotationDTO);
        Task<BaseResponse<bool>> DeleteShiftRotation(EntityShiftRotationDTO shiftRotationDTO);
        Task<BaseResponse<bool>> UpdateEntityShiftRotation(UpdateShiftRotationDTO shiftRotationDTO);
    }
}
