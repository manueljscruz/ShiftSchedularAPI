namespace ShiftSchedularEntity.Entities
{
    public class EntityRule
    {
        #region Properties

        public string EntityRuleId { get; set; }
        public int RuleTypeId { get; set; }
        public string RuleTypeDescription { get; set; }
        public string EntityId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual RuleType RuleType { get; set; }
        public virtual ICollection<EntityRuleSpecification> EntityRuleSpecifications { get; set; }
        public virtual Entity Entity { get; set; }

        #endregion
    }
}
