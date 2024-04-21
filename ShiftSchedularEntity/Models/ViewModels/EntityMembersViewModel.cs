using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class EntityMembersViewModel
    {
        public string EntityOwnerId { get; set; }
        public List<SkillLocalizedDTO> Skills { get; set; }
        public List<EntityWorkerMemberDTO> EntityMembers { get; set; }

        public EntityMembersViewModel()
        {
            Skills = new List<SkillLocalizedDTO>();
            EntityMembers = new List<EntityWorkerMemberDTO>();
        }
    }
}
