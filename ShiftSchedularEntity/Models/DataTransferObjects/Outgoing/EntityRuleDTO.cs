namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityRuleDTO
    {
        #region Properties

        public string EntityRuleId { get; set; }
        public int RuleTypeId { get; set; }
        public string RuleTypeDisplayValue { get; set; }
        public string RuleTypeDescription { get; set; }
        public string EntityId { get; set; }
        public List<EntityRuleSpecificationDTO> EntityRuleSpecificationDTOs { get; set; }

        #endregion

        #region Constructors

        public EntityRuleDTO()
        {
            EntityRuleId = string.Empty;
            RuleTypeId = 0;
            RuleTypeDisplayValue = string.Empty;
            RuleTypeDescription = string.Empty;
            EntityId = string.Empty;
            EntityRuleSpecificationDTOs = new List<EntityRuleSpecificationDTO>();
        }

        public EntityRuleDTO(string entityRuleId, int ruletTypeId, string ruleTypeDisplayValue, string ruleTypeDescription, string entityId, List<EntityRuleSpecificationDTO> entityRuleSpecificationDTOs)
        {
            EntityRuleId = entityRuleId;
            RuleTypeId = ruletTypeId;
            RuleTypeDisplayValue = ruleTypeDisplayValue;
            RuleTypeDescription = ruleTypeDescription;
            EntityId = entityId;
            EntityRuleSpecificationDTOs = entityRuleSpecificationDTOs;
        }

        #endregion
    }
}
