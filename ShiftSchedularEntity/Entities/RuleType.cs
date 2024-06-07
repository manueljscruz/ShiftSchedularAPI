using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class RuleType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RuleTypeId { get; set; }
        public string RuleTypeName { get; set; }

        #region Navigation Properties

        public virtual ICollection<EntityRule> EntityRules { get; set; }
        public virtual ICollection<RuleTypeLocalization> RuleTypeLocalizations { get; set; }
        public virtual ICollection<RuleTypeBusinessAspect> RuleTypeBusinessAspects { get; set; }

        #endregion
    }
}
