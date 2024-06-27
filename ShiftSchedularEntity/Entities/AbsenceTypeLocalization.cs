namespace ShiftSchedularEntity.Entities
{
    public class AbsenceTypeLocalization
    {
        public int AbsenceTypeId { get; set; }
        public int LocalizationId { get; set; }
        public string AbsenceTypeDisplayValue { get; set; }

        #region Navigation Properties

        public virtual Localization Localization { get; set; }
        public virtual AbsenceType AbsenceType { get; set; }

        #endregion
    }
}
