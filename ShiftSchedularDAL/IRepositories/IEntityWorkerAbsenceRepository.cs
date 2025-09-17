using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerAbsenceRepository : IGenericRepository<EntityWorkerAbsence>
    {
        Task<IEnumerable<EntityWorkerAbsence>> GetEntityWorkerAbsences(Guid entityId, string workerId, bool isOwner);
        Task<IEnumerable<EntityWorkerAbsence>> GetSpecificWorkerAbsences(Guid entityId, List<string> workers, DateTime? startDate = null, DateTime? endDate = null);
    }
}
