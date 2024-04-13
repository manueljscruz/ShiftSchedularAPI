namespace ShiftSchedularEntity.Entities
{
    public class SkillLocalization
    {
        #region Properties

        public int SkillId { get; set; }

        public int LocalizationId { get; set; }

        public string SkillDisplayValue { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Skill Skill { get; set; }

        public virtual Localization Localization { get; set; }

        #endregion
    }
}
