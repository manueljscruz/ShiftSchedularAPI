namespace ShiftSchedularEntity.Models.APIManagement
{
    public class BaseLocalizationSubmissionModel
    {
        public int LanguageId { get; set; }
        public string DisplayValue { get; set; }

        public BaseLocalizationSubmissionModel()
        {
            LanguageId = 0;
            DisplayValue = string.Empty;
        }

        public BaseLocalizationSubmissionModel(int languageId, string displayValue)
        {
            LanguageId = languageId;
            DisplayValue = displayValue;
        }
    }
}
