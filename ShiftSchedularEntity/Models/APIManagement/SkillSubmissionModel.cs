namespace ShiftSchedularEntity.Models.API_Management
{
    public class SkillSubmissionModel
    {
        #region Properties

        public string SkillName { get; set; }

        public string SkillHEXBgColor { get; set; }

        public string SkillHEXFontColor { get; set; }

        #endregion

        #region Constructors

        public SkillSubmissionModel()
        {
            SkillName = string.Empty;
            SkillHEXBgColor = string.Empty;
            SkillHEXFontColor = string.Empty;
        }

        public SkillSubmissionModel(string skillName, string hexBgColor, string hexFontColor)
        {
            SkillName = skillName;
            SkillHEXBgColor = hexBgColor;
            SkillHEXFontColor = hexFontColor;
        }

        #endregion
    }
}
