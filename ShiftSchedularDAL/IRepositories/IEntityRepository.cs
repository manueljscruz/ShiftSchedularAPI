using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityRepository : IGenericRepository<Entity>
    {
        Task<Entity> GetEntityById(Guid entityId, string languageCode);
        Task<List<Entity>> SearchEntitiesByName(string searchQuery, int localizationId);
    }
}
