using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityRule : BaseEntity
    {
        #region Properties

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityRuleId { get; set; }

        [Required]
        public int RuleTypeId { get; set; }

        public string RuleTypeDescription { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual RuleType RuleType { get; set; }
        public virtual ICollection<EntityRuleSpecification> EntityRuleSpecifications { get; set; }
        public virtual Entity Entity { get; set; }

        #endregion
    }
}
