using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.QueryModels;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerRepository : IGenericRepository<EntityWorker>
    {
        Task<IEnumerable<EntityWorkerDTO>> GetByWorkerId(string workerId);
        Task<IEnumerable<EntityWorker>> GetByEntityId(string entityId);
        Task<bool> IsWorkerInEntity(string entityId, string workerId);
        Task<List<EntityWorker>> GetByWorkerAndEntity(string workerId, string entityId);
        Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(string entityId);
        Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(string entityId, List<string> workers);
        Task<IEnumerable<int>> GetDistinctSkillsByEntityId(string entityId);
        Task<int> GetTotalCountByEntity(string entityId);
        Task<string> GetEntityOwnerId(string entityId);
        Task<bool> IsMemberOwner(string entityId, string workerId);
    }
}
