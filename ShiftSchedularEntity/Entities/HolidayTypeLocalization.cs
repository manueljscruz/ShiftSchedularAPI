using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class HolidayTypeLocalization
    {
        [Required]
        public int HolidayTypeId { get; set; }

        [Required]
        public int LocalizationId { get; set; }

        [MaxLength(100)]
        public string HolidayTypeDisplayValue { get; set; }

        #region Navigation Type

        public virtual HolidayType HolidayType { get; set; }
        public virtual Localization Localization { get; set; }

        #endregion
    }
}
