using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class ScheduleEntryBotIneligibility : BaseScheduleEntryIneligibility
    {
        [Required]
        public Guid UserBotId { get; set; }

        #region Navigation Properties

        public virtual UserBot UserBot { get; set; }

        #endregion
    }
}
