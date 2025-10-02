using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class ScheduleEntryWorkerIneligibility : BaseScheduleEntryIneligibility
    {
        [Required]
        public string ApplicationUserId { get; set; }

        #region Navigation Properties

        public virtual ApplicationUser ApplicationUser { get; set; }

        #endregion
    }
}
