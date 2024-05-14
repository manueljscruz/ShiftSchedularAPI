
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IShiftBreakTypeService
    {
        Task<ShiftBreakType> GetShiftBreakTypeById(int id);
        Task<IEnumerable<ShiftBreakType>> GetAllShiftBreakTypes();
        Task<List<ShiftBreakTypeLocalizedDTO>> GetAllShiftBreakTypesByLocalization(string lcode);
        Task<int> AddShiftBreakType(string strNewShiftBreakType);
        Task<bool> AddShiftBreakTypeLocalization(ShiftBreakTypeLocalizationSubmissionModel shiftBreakTypeLocalizationSubmissionModel);
        Task DeleteShiftBreakType(int id);
        Task UpdateShiftBreakType(ShiftBreakType shiftBreakType);
    }
}
