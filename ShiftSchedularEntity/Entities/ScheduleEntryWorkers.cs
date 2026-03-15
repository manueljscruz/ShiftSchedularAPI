using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShiftSchedularEntity.Entities.Base;

namespace ShiftSchedularEntity.Entities
{
    public class ScheduleEntryWorkers : BaseEntity
    {
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ScheduleEntryId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public string SpecificSkillAssignments { get; set; }

        #region Navigation Properties

        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ScheduleEntry ScheduleEntry { get; set; }

        #endregion
    }
}
