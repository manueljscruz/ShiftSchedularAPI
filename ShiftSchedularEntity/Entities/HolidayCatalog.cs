using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ShiftSchedularEntity.Entities
{
    public class HolidayCatalog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HolidayCatalogId { get; set; }

        [Required]
        public int HolidayTypeId { get; set; }


        [Required]
        public int HolidayBehaviourId { get; set; }

        [Required]
        [MaxLength(200)]
        public string HolidayName { get; set; }

        [MaxLength(500)]
        public string HolidayDescription { get; set; }

        [NotNull]
        [Range(1, 31)]
        public int RecurrenceDay { get; set; }

        [NotNull]
        [Range(1, 12)]
        public int RecurrenceMonth { get; set; }

        [DefaultValue(true)]
        public bool IsRecurring { get; set; }
        
        public int IsActive { get; set; }
        

        #region Navigation Properties

        public virtual HolidayType HolidayType { get; set; }
        public virtual HolidayBehaviour HolidayBehaviour { get; set; }
        public virtual ICollection<EntityHoliday> EntityHolidays { get; set; }
        public virtual ICollection<HolidayCatalogLocalization> HolidayCatalogLocalizations { get; set; }
        #endregion

    }
}
