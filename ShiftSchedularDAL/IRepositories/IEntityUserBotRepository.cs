using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.QueryModels;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityUserBotRepository : IGenericRepository<EntityUserBot>
    {
        Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctUserBotsByEntityId(Guid entityId);
        Task<int> GetUserBotsByEntityCount(Guid entityId);
        Task<IEnumerable<EntityUserBot>> GetEntityUserBotsByEntityAndId(Guid entityId, Guid userId);
        Task<IEnumerable<int>> GetDistinctSkillsByEntityId(Guid entityId);
    }
}
