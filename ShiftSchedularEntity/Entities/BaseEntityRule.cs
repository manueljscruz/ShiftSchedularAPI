using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    [Table("BaseEntityRules")]
    public class BaseEntityRule
    {
        public int BaseEntityRuleId { get; set; }
        public int RuleTypeId { get; set; }

        #region Navigation Properties

        public virtual IEnumerable<BaseEntityRuleSpecification> BaseEntityRuleSpecifications { get; set; }
        public virtual RuleType RuleType { get; set; }

        #endregion
    }
}
