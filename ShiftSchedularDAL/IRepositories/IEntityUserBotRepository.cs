using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.QueryModels;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityUserBotRepository : IGenericRepository<EntityUserBot>
    {
        /// <summary>
        /// Get all the user bots for a given entity.
        /// Used in the members page.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctUserBotsByEntityId(Guid entityId);

        /// <summary>
        /// Gets the count of user bots for a given entity.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<int> GetUserBotsByEntityCount(Guid entityId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<EntityUserBot> GetEntityUserBotByEntityAndId(Guid entityId, Guid userId);
        Task<IEnumerable<EntityUserBot>> GetUserBotsByEntityId(Guid entityId);
        Task<IEnumerable<int>> GetDistinctSkillsByEntityId(Guid entityId);
        Task<bool> DeleteEntityUserBot(Guid entityId, Guid userBotId);
    }
}
