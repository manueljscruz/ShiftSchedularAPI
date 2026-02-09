using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class HolidayCatalogLocalization
    {
        [Required]
        public int HolidayCatalogId { get; set; }

        [Required]
        public int LocalizationId { get; set; }

        [Required]
        [MaxLength(200)]
        public string LocalizedName { get; set; }

        [Required]
        [MaxLength(500)]
        public string LocalizedDescription { get; set; }

        public bool IsRecurring { get; set; }
        public int RecurrenceMonth { get; set; }
        public int RecurrenceDay { get; set; }
        public bool IsActive { get; set; }

        #region Navigation Properties

        public virtual HolidayCatalog HolidayCatalog { get; set; }
        public virtual Localization Localization { get; set; }

        #endregion
    }
}
