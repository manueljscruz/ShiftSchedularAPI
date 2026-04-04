using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Localization
    {
        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<GenderLocalization> GenderLocalizations { get; set; }
        public virtual ICollection<EntityTypeLocalization> EntityTypeLocalizations { get; set; }
        public virtual ICollection<SkillLocalization> SkillLocalizations { get; set; }
        public virtual ICollection<ShiftBreakTypeLocalization> ShiftBreakTypeLocalizations { get; set; }
        public virtual ICollection<RuleTypeLocalization> RuleTypeLocalizations { get; set; }
        public virtual ICollection<BusinessAspectLocalization> BusinessAspectLocalizations { get; set; }
        public virtual ICollection<AbsenceTypeLocalization> AbsenceTypeLocalizations { get; set; }
        public virtual ICollection<HolidayTypeLocalization> HolidayTypeLocalizations { get; set; }
        public virtual ICollection<HolidayBehaviourLocalization> HolidayBehaviourLocalizations { get; set; }
        public virtual ICollection<HolidayCatalogLocalization> HolidayCatalogLocalizations { get; set; }
        public virtual ICollection<EntityPermissionRoleLocalization> EntityPermissionRoleLocalizations { get; set; }
        public virtual ICollection<NotificationTypeLocalization> NotificationTypeLocalizations { get; set; }

        #endregion

    }
}
