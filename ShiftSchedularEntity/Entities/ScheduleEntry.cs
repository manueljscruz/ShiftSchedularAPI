using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    [Table("ScheduleEntry")]
    public class ScheduleEntry
    {
        public string ScheduleEntryId { get; set; }
        public string ShiftId { get; set; }
        public DateTime ScheduleStartDate { get; set; }
        public DateTime ScheduleEndDate { get; set; }

        #region Navigation Properties

        public virtual Shift Shift { get; set; }
        public virtual IEnumerable<ScheduleEntryWorkers> ScheduleEntryWorkers { get; set; }

        #endregion
    }
}
