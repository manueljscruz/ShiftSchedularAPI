using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerShiftAssignedsRepository : IGenericRepository<EntityWorkerShiftAssigned>
    {
        Task<bool> DeleteAllByEntityIdAndUserId(Guid entityId, Guid workerGuid);
        Task<IEnumerable<EntityWorkerShiftAssigned>> GetAllByEntityIdAndUserId(Guid entityId, Guid userId);
        Task<IEnumerable<EntityWorkerShiftAssigned>> GetByEntityIdAndShiftId(Guid entityId, Guid shiftId);
    }
}
