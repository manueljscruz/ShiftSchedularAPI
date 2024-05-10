
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IShiftService
    {
        Task<BaseResponse<ShiftDTO>> AddEntityShift(AddShiftDTO shiftDTO);
        Task<BaseResponse<ShiftBreakDTO>> AddEntityShiftBreak(AddShiftBreakDTO shiftBreakDTO);
        Task<BaseResponse<bool>> DeleteEntityShift(string entityId, string shiftId);
        Task<BaseResponse<bool>> DeleteEntityShiftBreak(DeleteEntityShiftBreakDTO deleteEntityShiftBreak);
        Task<IEnumerable<ShiftDTO>> GetAllEntityShifts(string entityId, string lcode);
        Task<ShiftDTO> GetShiftById(string id);
        Task<BaseResponse<bool>> UpdateEntityShift(ShiftDTO shift);
        Task<BaseResponse<bool>> UpdateEntityShiftBreak(ShiftBreakDTO shiftBreak);
    }
}
