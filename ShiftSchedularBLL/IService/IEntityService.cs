using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityService
    {
        Task<Entity> GetEntityById(string entityId);
        Task<IEnumerable<Entity>> GetAllEntities();
        Task<BaseResponse<Entity>> AddEntity(NewEntityDTO newEntity);
        Task<BaseResponse<bool>> UpdateEntity(Entity entity);
        Task<BaseResponse<bool>> DeleteEntityById(string entityId);
        Task<List<EntityWorkerDTO>> GetEntitiesByWorkerId(string workerId);
    }
}
