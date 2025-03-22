using System.Web;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddNewMemberDTO
    {
        public bool IsBot { get; set; }

        public Guid DestinationEntityId { get; set; }

        public string MemberName { get; set; }

        public string MemberEmail { get; set; }

        public List<SkillLocalizedDTO> AssignedSkills { get; set; }

        public string LanguageCode { get; set; }
    }
}
