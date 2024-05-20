using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IShiftTemplateBreaksRepository : IGenericRepository<ShiftTemplateBreaks>
    {
        Task<IEnumerable<ShiftTemplateBreaks>> GetShiftTemplateBreaksByBreakId(int breakId);
        Task<IEnumerable<ShiftTemplateBreaks>> GetShiftTemplateBreaksByShiftId(int shiftId);
        Task DeleteShiftTemplateBreak (ShiftTemplateBreaks shiftTemplateInstance);
    }
}
