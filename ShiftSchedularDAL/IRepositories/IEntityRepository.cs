using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityRepository : IGenericRepository<Entity>
    {
        public Task<IEnumerable<Entity>> GetEntitiesByIdRange(IEnumerable<string> entitiesIds);
    }
}
