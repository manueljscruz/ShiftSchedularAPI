using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class HolidayBehaviour
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HolidayBehaviourId { get; set; }

        [Required]
        [MaxLength(100)]
        public string HolidayBehaviourName { get; set; }

        [DefaultValue(true)]
        public bool AllowsOperatingTimes { get; set; }

        #region Navigation Properties

        public virtual ICollection<HolidayBehaviourLocalization> HolidayBehaviourLocalizations { get; set; }
        public virtual ICollection<HolidayCatalog> HolidayCatalogs { get; set; }
        public virtual ICollection<EntityHoliday> EntityHolidays { get; set; }

        #endregion
    }
}
