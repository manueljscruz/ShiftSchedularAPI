using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities.Base
{
    public class BaseScheduleEntryIneligibility
    {
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ScheduleEntryId { get; set; }
        public string IneligibilityObservations { get; set; }
        public DateTime DateOfAssessement { get; set; }

        #region Navigation Properties
        public virtual ScheduleEntry ScheduleEntry { get; set; }

        #endregion
    }
}
