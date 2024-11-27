using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class ScheduleEntryWorkers
    {
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ScheduleEntryId { get; set; }
        public string ApplicationUserId { get; set; }

        #region Navigation Properties

        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ScheduleEntry ScheduleEntry { get; set; }

        #endregion
    }
}
