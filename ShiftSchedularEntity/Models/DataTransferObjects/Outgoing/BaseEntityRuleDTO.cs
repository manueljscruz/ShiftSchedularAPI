using ShiftSchedularEntity.Entities;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class BaseEntityRuleDTO
    {
        public int BaseEntityRuleId { get; set; }
        public int RuleTypeId { get; set; }
        public string RuleTypeDisplayValue { get; set; }
        public List<BaseEntityRuleSpecification> BaseEntityRuleSpecifications { get; set; }

        public BaseEntityRuleDTO()
        {
            BaseEntityRuleSpecifications = new List<BaseEntityRuleSpecification>();
        }
    }
}
