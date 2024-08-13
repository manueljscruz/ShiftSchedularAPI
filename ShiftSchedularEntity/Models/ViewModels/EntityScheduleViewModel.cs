using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class EntityScheduleViewModel
    {
        public List<ScheduleEntryDTO> ScheduleEntries { get; set; }
        public bool AllowEdit { get; set; }
        public IEnumerable<EntityWorkerMemberDTO> EntityWorkerMembers { get; set; }
        public IEnumerable<ShiftDTO> Shifts { get; set; }
        public IEnumerable<EntityRuleDTO> EntityRules { get; set; }

        public EntityScheduleViewModel()
        {
            ScheduleEntries = new List<ScheduleEntryDTO>();
            AllowEdit = false;
            EntityWorkerMembers = new List<EntityWorkerMemberDTO>();
            EntityRules = new List<EntityRuleDTO>();
            Shifts = new List<ShiftDTO>();
        }
    }
}
