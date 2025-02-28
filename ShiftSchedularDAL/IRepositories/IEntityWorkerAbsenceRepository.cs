using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerAbsenceRepository : IGenericRepository<EntityWorkerAbsence>
    {
        Task<IEnumerable<EntityWorkerAbsence>> GetEntityWorkerAbsences(Guid entityId, string workerId, bool isOwner);
    }
}
