namespace ShiftSchedularEntity.Entities
{
    public class EntityWorkerAbsence
    {
        public string EntityWorkerAbsenceId { get; set; }
        public string WorkerId { get; set; }
        public string EntityId { get; set; }
        public int AbsenceTypeId { get; set; }
        public string Observations { get; set; }
        public DateTime AbsenceStartDate { get; set; }
        public DateTime AbsenceEndDate { get; set; }
        public bool AbsenceApproved { get; set; }
        public DateTime AbsenceDateDecision { get; set; }

        #region Navigation Properties

        public virtual Worker Worker { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual AbsenceType AbsenceType { get; set; }

        #endregion
    }
}
