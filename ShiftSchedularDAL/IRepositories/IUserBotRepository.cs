using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IUserBotRepository : IGenericRepository<UserBot>
    {
        Task<IEnumerable<UserBot>> GetUserBotsByEntityId(Guid entityId);
    }
}
