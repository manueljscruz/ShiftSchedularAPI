using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerAbsenceRepository : IGenericRepository<EntityWorkerAbsence>
    {
        Task<IEnumerable<EntityWorkerAbsence>> GetEntityWorkerAbsences(string entityId, string workerId, bool isOwner);
    }
}
