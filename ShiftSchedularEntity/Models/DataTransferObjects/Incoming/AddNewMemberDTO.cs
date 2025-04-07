using System.Web;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddNewMemberDTO
    {
        public bool IsBot { get; set; }

        public Guid DestinationEntityId { get; set; }

        public string MemberName { get; set; }

        public string MemberEmail { get; set; }

        public List<SkillLocalizedDTO> AssignedSkills { get; set; }
        public bool PartOfRotation { get; set; }
        public List<ShiftDTO> AssignedShifts { get; set; }
        public string LanguageCode { get; set; }
    }
}
