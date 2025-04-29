using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class ScheduleEntryBots
    {
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ScheduleEntryId { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid UserBotId { get; set; }

        public string SpecificSkillAssignments { get; set; }

        #region Navigation Properties

        public virtual UserBot UserBot { get; set; }
        public virtual ScheduleEntry ScheduleEntry { get; set; }

        #endregion
    }
}
