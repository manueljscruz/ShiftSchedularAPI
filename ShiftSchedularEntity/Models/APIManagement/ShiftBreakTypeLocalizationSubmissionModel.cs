namespace ShiftSchedularEntity.Models.APIManagement
{
    public class ShiftBreakTypeLocalizationSubmissionModel
    {
        #region Properties

        public int ShiftBreakTypeId { get; set; }
        public string ShiftBreakTypeDisplayValue { get; set; }
        public int LanguageId { get; set; }

        #endregion

        #region Constructors

        public ShiftBreakTypeLocalizationSubmissionModel()
        {
            ShiftBreakTypeId = 0;
            ShiftBreakTypeDisplayValue = string.Empty;
            LanguageId = 0;
        }

        public ShiftBreakTypeLocalizationSubmissionModel(int shiftBreakTypeId, string shiftBreakTypeDisplayValue, int languageId)
        {
            ShiftBreakTypeId = shiftBreakTypeId;
            ShiftBreakTypeDisplayValue = shiftBreakTypeDisplayValue;
            LanguageId = languageId;
        }

        #endregion
    }
}
