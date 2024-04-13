using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.QueryModels;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerRepository : IGenericRepository<EntityWorker>
    {
        Task<IEnumerable<EntityWorkerDTO>> GetByWorkerId(string workerId);

        Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(string entityId);
    }
}
