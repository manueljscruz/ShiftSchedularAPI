using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IShiftRepository : IGenericRepository<Shift>
    {
        Task<Shift> GetShiftById(Guid shiftId);
        Task<IEnumerable<Shift>> GetEntityShifts(Guid entityId);
        Task<IEnumerable<Shift>> GetEntityShifts(List<string> shiftIdentifiers);
    }
}
