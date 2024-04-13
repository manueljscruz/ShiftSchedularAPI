namespace ShiftSchedularEntity.Models.API_Management
{
    public class SkillLocalizationSubmissionModel
    {
        #region Properties

        public int SkillId { get; set; }
        public int LanguageId { get; set; }
        public string SkillDisplayValue { get; set; }

        #endregion

        #region Constructors

        public SkillLocalizationSubmissionModel()
        {
            SkillId = 0;
            LanguageId = 0;
            SkillDisplayValue = string.Empty;
        }

        public SkillLocalizationSubmissionModel(int skillId, int languageId, string skillDisplayValue)
        {
            SkillId = skillId;
            LanguageId = languageId;
            SkillDisplayValue = skillDisplayValue;
        }

        #endregion
    }
}
