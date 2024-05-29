using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class BusinessAspect
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BusinessAspectId { get; set; }
        public string BusinessAspectName { get; set; }

        #region Navigation Properties

        public virtual ICollection<EntityRuleSpecification> EntityRuleSpecifications { get; set; }

        public virtual ICollection<BusinessAspectLocalization> BusinessAspectLocalizations { get; set; }

        #endregion
    }
}
