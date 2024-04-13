namespace ShiftSchedularEntity.Models.DataTransferObjects
{
    public class SkillLocalizedDTO
    {
        #region Properties

        public int SkillId { get; set; }
        public string SkillLocalizedName { get; set; }
        public string SkillHexBGColor { get; set; }
        public string SkillHexFontColor { get; set; }

        #endregion

        #region Constructor

        public SkillLocalizedDTO()
        {
            SkillId = 0;
            SkillLocalizedName = string.Empty;
            SkillHexBGColor = string.Empty;
            SkillHexFontColor = string.Empty;
        }

        #endregion
    }
}
