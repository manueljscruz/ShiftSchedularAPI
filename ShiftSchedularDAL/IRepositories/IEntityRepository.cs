using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityRepository : IGenericRepository<Entity>
    {
        Task<Entity> GetEntityById(Guid entityId, string languageCode);
        Task<List<Entity>> SearchEntitiesByName(string searchQuery, int localizationId);
        Task<List<Entity>> GetChildEntities(Guid parentEntityId, string languageCode);
        Task<List<Entity>> GetAncestorChain(Guid entityId);
        Task<List<Entity>> GetEntitiesByIds(List<Guid> entityIds);
        Task<Guid> GetRootEntityId(Guid entityId);
    }
}
