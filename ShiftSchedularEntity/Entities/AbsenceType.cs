namespace ShiftSchedularEntity.Entities
{
    public class AbsenceType
    {
        public int AbsenceTypeId { get; set; }
        public string AbsenceTypeName { get; set; }

        #region Navigation Properties

        public virtual ICollection<AbsenceTypeLocalization> AbsenceTypeLocalizations { get; set; }
        public virtual ICollection<EntityWorkerAbsence> EntityWorkerAbsences { get; set; }

        #endregion
    }
}
