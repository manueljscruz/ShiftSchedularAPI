namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityWorkerAbsenceDTO
    {
        public Guid EntityWorkerAbsenceId { get; set; }
        public Guid EntityId { get; set; }
        public string WorkerId { get; set; }
        public int AbsenceTypeId { get; set; }
        public string AbsenceTypeDisplayValue { get; set; }
        public string Observations { get; set; }
        public DateTime AbsenceStartDate { get; set; }
        public DateTime AbsenceEndDate { get; set; }
        public TimeSpan DateOffset { get; set; }
        public bool AbsenceApproved { get; set; }
        public string AbsenceDecisionOwner { get; set; }
        public string AbsenceApproverName { get; set; }
        public DateTime AbsenceDateDecision { get; set; }
        public TimeSpan AbsenceDateDecisionOffset { get; set; }
    }
}
