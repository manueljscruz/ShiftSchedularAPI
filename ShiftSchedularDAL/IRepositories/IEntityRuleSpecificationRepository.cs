using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityRuleSpecificationRepository
    {
        Task<EntityRuleSpecification> AddEntityRuleSpecification(EntityRuleSpecification entityRuleSpecification);
        Task<bool> UpdateEntityRuleSpecification(EntityRuleSpecification entityRuleSpecification);
        Task<bool> DeleteEntityRuleSpecification(string entityRuleId, int specId);
        Task<bool> DeleteEntityRuleSpecificationsByRuleId(string entityRuleId);
        Task<EntityRuleSpecification> GetEntityRuleSpecification(string entityRuleId, int specificationId);
        Task<IEnumerable<EntityRuleSpecification>> GetEntityRuleSpecifications(string entityRuleId);
    }
}
