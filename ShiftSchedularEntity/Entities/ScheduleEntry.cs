namespace ShiftSchedularEntity.Entities
{
    public class ScheduleEntry
    {
        public string ScheduleEntryId { get; set; }
        public string ShiftId { get; set; }
        public string EntityId { get; set; }
        public DateTime ScheduleStartDate { get; set; }
        public DateTime ScheduleEndDate { get; set; }

        #region Navigation Properties

        public virtual Shift Shift { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual IEnumerable<ScheduleEntryWorkers> ScheduleEntryWorkers { get; set; }

        #endregion
    }
}
