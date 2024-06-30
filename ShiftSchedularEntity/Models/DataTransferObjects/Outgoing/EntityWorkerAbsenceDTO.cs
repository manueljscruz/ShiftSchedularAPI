namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityWorkerAbsenceDTO
    {
        public string EntityWorkerAbsenceId { get; set; }
        public string EntityId { get; set; }
        public string WorkerId { get; set; }
        public int AbsenceTypeId { get; set; }
        public string AbsenceTypeDisplayValue { get; set; }
        public string Observations { get; set; }
        public DateTime AbsenceStartDate { get; set; }
        public DateTime AbsenceEndDate { get; set; }
        public bool AbsenceApproved { get; set; }
        public string AbsenceDecisionOwner { get; set; }
        public DateTime AbsenceDateDecision { get; set; }
    }
}
