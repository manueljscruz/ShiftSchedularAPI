using ShiftSchedularEntity.Models.DataAnnotationModels;
using ShiftSchedularRL.Resources.Dashboard;
using ShiftSchedularRL.Resources.MemberManagement;
using ShiftSchedularRL.Resources.ScheduleManagement;
using ShiftSchedularRL.Resources.ShiftManagement;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AssignEntryDTO
    {
        [RequiredWithResourceMessage(typeof(EntityWorkerRelatedMessages), "MemberIdentifierEmpty")]
        public string WorkerId { get; set; }

        [RequiredWithResourceMessage(typeof(EntitiesRelatedMessages), "EntityNoIdentifierError")]
        public Guid EntityId { get; set; }

        public string LanguageCode { get; set; }

        public bool IsBot { get; set; }
        public string ScheduleId { get; set; }

        [RequiredWithResourceMessage(typeof(ShiftRelatedMessages), "ShiftIdIsNull")]
        public Guid ShiftId { get; set; }

        [RequiredWithResourceMessage(typeof(ScheduleRelatedMessages), "DateInvalidError")]
        public DateTime Date { get; set; }
    }
}
