using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class HolidayType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HolidayTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string HolidayTypeName { get; set; }

        #region Navigation Properties

        public virtual ICollection<HolidayTypeLocalization> HolidayTypeLocalizations { get; set; }
        public virtual ICollection<HolidayCatalog> HolidayCatalogs { get; set; }

        #endregion
    }
}
