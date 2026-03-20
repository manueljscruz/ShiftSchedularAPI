using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IShiftBreakRepository : IGenericRepository<ShiftBreak>
    {
        Task<IEnumerable<ShiftBreak>> GetBreaksByShiftId(Guid shiftId);

        /// <summary>
        /// Retrieves all shift breaks associated with a specific entity.
        /// Intended for bulk operations such as deletion.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity</param>
        /// <returns>A collection of ShiftBreak records for the specified entity</returns>
        Task<IEnumerable<ShiftBreak>> GetByEntityId(Guid entityId);
    }
}
