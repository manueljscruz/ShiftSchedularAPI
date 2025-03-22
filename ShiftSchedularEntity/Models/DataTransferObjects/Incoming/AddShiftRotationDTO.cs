using ShiftSchedularEntity.Models.DataAnnotationModels;
using ShiftSchedularRL.Resources.Home;
using ShiftSchedularRL.Resources.ShiftManagement;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddShiftRotationDTO
    {
        [RequiredWithResourceMessage(typeof(ShiftRelatedMessages), "ShiftEntityIdIsNull")]
        public Guid EntityId { get; set; }
        public Guid ShiftId { get; set; }
        public bool IsLeave { get; set; }
        public TimeSpan LeaveDuration { get; set; }
    }
}
