using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.API_Management;
using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityTypeService
    {
        Task<EntityType> GetEntityTypeById(int entityTypeId);
        Task<IEnumerable<EntityType>> GetAllEntityTypes();
        Task<int> AddEntityType(string strNewEntityTypeValue);
        Task UpdateEntityType(EntityType entityType);
        Task DeleteEntityType(int entityTypeId);
        Task<bool> AddEntityTypeLocalization(EntityTypeLocalizationSubmissionModel entityTypeLocalizationSubmission);
        Task<List<EntityTypeLocalizedDTO>> GetAllEntityTypesByLocalization(string lcode);
    }
}
