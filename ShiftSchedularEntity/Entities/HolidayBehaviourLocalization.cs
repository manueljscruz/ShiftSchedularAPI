using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class HolidayBehaviourLocalization
    {
        [Required]
        public int HolidayBehaviourId { get; set; }

        [Required]
        public int LocalizationId { get; set; }

        [MaxLength(100)]
        public string HolidayBehaviourDisplayValue { get; set; }

        #region Navigation Properties

        public virtual HolidayBehaviour HolidayBehaviour { get; set; }
        public virtual Localization Localization { get; set; }

        #endregion
    }
}
