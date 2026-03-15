using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class UserBot : BaseEntity
    {
        #region Properties

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid UserBotId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string UserDisplayName { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<EntityUserBot> EntityUserBots { get; set; }
        public virtual ICollection<EntityUserBotSkill> EntityUserBotSkills { get; set; } 
        public virtual ICollection<EntityUserBotShiftAssigned> EntityUserBotShiftAssigneds { get; set; } 
        public virtual ICollection<ScheduleEntryBots> ScheduleEntryBots { get; set; } 
        public virtual ICollection<ScheduleEntryBotIneligibility> ScheduleEntryBotIneligibilities { get; set; } 
        #endregion
    }
}
