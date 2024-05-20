using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IShiftTemplateService
    {
        Task<ShiftTemplate> GetShiftTemplateById(int id);
        Task<List<ShiftBreakTemplateDTO>> GetShiftBreakTemplates(string lcode);
        Task<BaseResponse<int>> AddShiftTemplate(ShiftTemplateSubmissionModel submissionModel);
        Task<BaseResponse<int>> AddShiftBreakTemplate(ShiftBreakTemplateSubmissionModel submissionModel);
        Task<BaseResponse<bool>> UpdateShiftTemplate(ShiftTemplateUpdateDTO shiftTemplateUpdateDTO);
        Task<BaseResponse<bool>> UpdateShiftBreakTemplate(ShiftBreakTemplateDTO shiftBreakTemplateDTO);
        Task<BaseResponse<bool>> UpdateShiftTemplatePopCount(int shiftTemplateId);
        Task<BaseResponse<bool>> UpdateShiftBreakTemplatePopCount(int shiftBreakTemplateId);
        Task<BaseResponse<bool>> DeleteShiftTemplate(int shiftTemplateId);
        Task<BaseResponse<bool>> DeleteShiftBreakTemplate(int shiftBreakTemplateId);
    }
}
