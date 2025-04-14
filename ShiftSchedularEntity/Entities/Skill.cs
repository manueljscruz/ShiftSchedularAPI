using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Skill
    {
        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SkillId { get; set; }

        public string SkillName { get; set; }

        public string HexBGColor { get; set; }

        public string HexFontColor { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<EntityWorkerSkill> EntityWorkerSkills { get; set; }
        public virtual ICollection<SkillLocalization> SkillLocalizations { get; set; }
        public virtual ICollection<EntityUserBotSkill> EntityUserBotSkills { get; set; }

        #endregion
    }
}
