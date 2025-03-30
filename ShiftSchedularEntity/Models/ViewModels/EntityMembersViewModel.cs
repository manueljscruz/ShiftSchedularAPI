using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class EntityMembersViewModel
    {
        public string EntityOwnerId { get; set; }
        public List<SkillLocalizedDTO> Skills { get; set; }
        public IEnumerable<ShiftDTO> Shifts { get; set; }
        public List<EntityWorkerMemberDTO> EntityMembers { get; set; }

        public EntityMembersViewModel()
        {
            Skills = new List<SkillLocalizedDTO>();
            Shifts = new List<ShiftDTO>();
            EntityMembers = new List<EntityWorkerMemberDTO>();
        }
    }
}
