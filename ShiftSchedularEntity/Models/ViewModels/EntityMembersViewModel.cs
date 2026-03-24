using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class EntityMembersViewModel
    {
        public string EntityOwnerId { get; set; }
        public List<SkillLocalizedDTO> Skills { get; set; }
        public List<SkillLocalizedDTO> EntityUsedSkills { get; set; }
        public IEnumerable<ShiftDTO> Shifts { get; set; }
        public PagedList<EntityWorkerMemberDTO> EntityMembers { get; set; }
        public List<EntityPermissionRoleDTO> EntityPermissionRoles { get; set; }

        public EntityMembersViewModel()
        {
            Skills = new List<SkillLocalizedDTO>();
            EntityUsedSkills = new List<SkillLocalizedDTO>();
            Shifts = new List<ShiftDTO>();
            EntityMembers = PagedList<EntityWorkerMemberDTO>.CreateEmpty();
            EntityPermissionRoles = new List<EntityPermissionRoleDTO>();
        }
    }
}
