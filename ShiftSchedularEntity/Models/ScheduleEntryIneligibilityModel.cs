namespace ShiftSchedularEntity.Models
{
    public class ScheduleEntryIneligibilityModel
    {
        public Guid ScheduleEntryId { get; set; }
        public string IneligibilityObservations { get; set; }
        public DateTime DateOfAssessment { get; set; }
        public string WorkerId { get; set; }
        public bool IsBot { get; set; }
    }
}
