namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddEntityWorkerAbsenceDTO
    {
        public string WorkerId { get; set; }
        public Guid EntityId { get; set; }
        public int AbsenceTypeId { get; set; }
        public string Observations { get; set; }
        public DateTime AbsenceStartDate { get; set; }
        public DateTime AbsenceEndDate { get; set; }
        public int OffsetMinutes { get; set; }
        public string TimezoneId { get; set; }
        public bool IsFullDay { get; set; }
    }
}
