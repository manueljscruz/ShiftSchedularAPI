using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.QueryModels;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerRepository : IGenericRepository<EntityWorker>
    {
        Task<IEnumerable<EntityWorkerDTO>> GetByWorkerId(string workerId);
        Task<IEnumerable<EntityWorker>> GetByEntityId(Guid entityId);
        Task<bool> IsWorkerInEntity(Guid entityId, string workerId);
        Task<EntityWorker> GetByWorkerAndEntity(string workerId, Guid entityId);
        Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(Guid entityId);
        Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(Guid entityId, List<string> workers);
        Task<IEnumerable<int>> GetDistinctSkillsByEntityId(Guid entityId);
        Task<int> GetTotalCountByEntity(Guid entityId);
        Task<string> GetEntityOwnerId(Guid entityId);
        Task<bool> IsMemberOwner(Guid entityId, string workerId);
    }
}
