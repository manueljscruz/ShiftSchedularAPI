namespace ShiftSchedularEntity.Entities
{
    public class ShiftBreakTypeLocalization
    {
        #region Properties

        public int LocalizationId { get; set; }
        public int ShiftBreakTypeId { get; set; }
        public string ShiftBreakTypeDisplayValue { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ShiftBreakType ShiftBreakType { get; set; }

        public virtual Localization Localization { get; set; }

        #endregion
    }
}
