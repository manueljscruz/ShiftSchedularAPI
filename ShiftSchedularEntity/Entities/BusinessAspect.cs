using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class BusinessAspect
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BusinessAspectId { get; set; }
        public string BusinessAspectName { get; set; }

        #region Navigation Properties
        public virtual ICollection<BusinessAspectLocalization> BusinessAspectLocalizations { get; set; }
        public virtual ICollection<RuleTypeBusinessAspect> RuleTypeBusinessAspects { get; set; }

        #endregion
    }
}
