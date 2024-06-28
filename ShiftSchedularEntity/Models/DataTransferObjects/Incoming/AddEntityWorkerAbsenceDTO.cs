namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddEntityWorkerAbsenceDTO
    {
        public string WorkerId { get; set; }
        public string EntityId { get; set; }
        public int AbsenceTypeId { get; set; }
        public string Observations { get; set; }
        public DateTime AbsenceStartDate { get; set; }
        public DateTime AbsenceEndDate { get; set; }
    }
}
