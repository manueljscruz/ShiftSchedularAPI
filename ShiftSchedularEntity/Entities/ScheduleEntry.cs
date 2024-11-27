using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    [Table("ScheduleEntry")]
    public class ScheduleEntry
    {
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ScheduleEntryId { get; set; }
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ShiftId { get; set; }
        public DateTime ScheduleStartDate { get; set; }
        public DateTime ScheduleEndDate { get; set; }

        #region Navigation Properties

        public virtual Shift Shift { get; set; }
        public virtual IEnumerable<ScheduleEntryWorkers> ScheduleEntryWorkers { get; set; }

        #endregion
    }
}
