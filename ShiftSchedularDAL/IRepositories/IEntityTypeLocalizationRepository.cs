using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityTypeLocalizationRepository : IGenericRepository<EntityTypeLocalization>
    {
        Task<EntityTypeLocalization> GetEntityTypeLocalizationByIds(int entityTypeId, string languageCode);
    }
}
