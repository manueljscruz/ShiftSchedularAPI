
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
        Task<BaseResponse<bool>> DeleteEntityShiftBreak(DeleteEntityShiftBreakDTO deleteEntityShiftBreak);
        Task<ShiftViewModel> GetEntityShiftsViewModel(EntityShiftViewModelRequestDTO shiftViewModelRequestDTO);
        Task<ShiftDTO> GetShiftById(string shiftId, string lcode);
        Task<BaseResponse<bool>> UpdateEntityShift(ShiftDTO shift);
        Task<BaseResponse<bool>> UpdateEntityShiftBreak(ShiftBreakDTO shiftBreak);
    }
}
