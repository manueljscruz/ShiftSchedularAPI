using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityHoliday
    {
        [Key]
        public Guid EntityHolidayId { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        public int? HolidayCatalogId { get; set; }

        [Required]
        public int HolidayBehaviourId { get; set; }

        [MaxLength(200)]
        public string CustomHolidayName { get; set; }

        [Range(0,31)]
        public int CustomDay { get; set; }

        [Range(1, 12)]
        public int CustomMonth { get; set; }

        public TimeSpan? OperatingStartTime { get; set; }

        public TimeSpan? OperatingEndTime { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual HolidayCatalog? HolidayCatalog { get; set; }
        public virtual HolidayBehaviour HolidayBehaviour { get; set; }

        #endregion
    }
}
