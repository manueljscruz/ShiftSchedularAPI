namespace ShiftSchedularEntity.Entities
{
    public class BusinessAspectLocalization
    {
        public int BusinessAspectId { get; set; }
        public int LocalizationId { get; set; }
        public string BusinessAspectDisplayValue { get; set; }

        #region Navigation Properties

        public virtual Localization Localization { get; set; }
        public virtual BusinessAspect BusinessAspect { get; set; }

        #endregion
    }
}
