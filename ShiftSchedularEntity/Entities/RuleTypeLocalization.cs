namespace ShiftSchedularEntity.Entities
{
    public class RuleTypeLocalization
    {
        public int RuleTypeId { get; set; }
        public int LocalizationId { get; set; }
        public string RuleTypeDisplayValue { get; set; }

        #region Navigation Properties

        public virtual Localization Localization { get; set; }
        public virtual RuleType RuleType { get; set; }

        #endregion
    }
}
