namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityRuleDTO
    {
        #region Properties

        public Guid EntityRuleId { get; set; }
        public int RuleTypeId { get; set; }
        public bool IsSpecValueBoolean { get; set; }
        public string RuleTypeDisplayValue { get; set; }
        public string RuleTypeDescription { get; set; }
        public Guid EntityId { get; set; }
        public List<EntityRuleSpecificationDTO> EntityRuleSpecificationDTOs { get; set; }

        #endregion

        #region Constructors

        public EntityRuleDTO()
        {
            EntityRuleId = Guid.Empty;
            RuleTypeId = 0;
            RuleTypeDisplayValue = string.Empty;
            RuleTypeDescription = string.Empty;
            EntityId = Guid.Empty;
            EntityRuleSpecificationDTOs = new List<EntityRuleSpecificationDTO>();
        }

        public EntityRuleDTO(Guid entityRuleId, int ruletTypeId, string ruleTypeDisplayValue, string ruleTypeDescription, Guid entityId, List<EntityRuleSpecificationDTO> entityRuleSpecificationDTOs)
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
