using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityRuleSpecificationRepository
    {
        Task<EntityRuleSpecification> AddEntityRuleSpecification(EntityRuleSpecification entityRuleSpecification);
        Task<bool> UpdateEntityRuleSpecification(EntityRuleSpecification entityRuleSpecification);
        Task<bool> DeleteEntityRuleSpecification(Guid entityRuleId, int specId);
        Task<bool> DeleteEntityRuleSpecificationsByRuleId(Guid entityRuleId);
        Task<bool> DeleteRange(IEnumerable<EntityRuleSpecification> entityRuleSpecifications);
        Task<EntityRuleSpecification> GetEntityRuleSpecification(Guid entityRuleId, int specificationId);
        Task<IEnumerable<EntityRuleSpecification>> GetEntityRuleSpecifications(Guid entityRuleId);
    }
}
