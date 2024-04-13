namespace ShiftSchedularEntity.Models
{
    public class GenderLocalizationSubmissionModel
    {
        #region Properties

        public int GenderId { get; set; }

        public int LanguageId { get; set; }

        public string GenderDisplayValue { get; set; }

        #endregion

        #region Constructors

        public GenderLocalizationSubmissionModel()
        {
            GenderId = 0;
            LanguageId = 0;
            GenderDisplayValue = string.Empty;
        }

        public GenderLocalizationSubmissionModel(int genderId, int languageId, string genderDisplayValue)
        {
            GenderId = genderId;
            LanguageId = languageId;
            GenderDisplayValue = genderDisplayValue;
        }

        #endregion
    }
}
