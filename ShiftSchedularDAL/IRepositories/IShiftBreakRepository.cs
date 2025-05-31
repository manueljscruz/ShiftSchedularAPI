using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IShiftBreakRepository : IGenericRepository<ShiftBreak>
    {
        Task<IEnumerable<ShiftBreak>> GetBreaksByShiftId(Guid shiftId);
    }
}
