using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class MemberListFilterDTO
    {
        public string? NameFilter { get; set; }
        public bool ApplyMemberTypeFilter { get; set; }
        public bool IsBot { get; set; }
        public List<SkillLocalizedDTO>? SelectedSkills { get; set; }
        public List<ShiftDTO>? SelectedShits { get; set; }
        public bool PartOfRotation { get; set; }
        public bool WorkWeekDays { get; set; }
        public bool WorkWeekEnds { get; set; }
    }
}
