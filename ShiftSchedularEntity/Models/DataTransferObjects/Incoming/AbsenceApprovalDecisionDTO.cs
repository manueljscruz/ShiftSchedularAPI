namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AbsenceApprovalDecisionDTO
    {
        public string EntityWorkerAbsenceId { get; set; }
        public string AbsenceDecisionSignature { get; set; }
        public bool AbsenceDecision { get; set; }
        public string LanguageCode { get; set; }
    }
}
