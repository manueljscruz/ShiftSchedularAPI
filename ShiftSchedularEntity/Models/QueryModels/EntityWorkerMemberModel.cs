using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Models.QueryModels
{
    public class EntityWorkerMemberModel
    {
        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public bool IsBot { get; set; }
        public bool PartOfRotation { get; set; }
        public bool WorksWeekDays { get; set; }
        public bool WorksWeekends { get; set; }
        public bool MultipleShiftAssignments { get; set; }
        public DateTime DateOfJoin { get; set; }
        public string SkillIds { get; set; }
        public bool IsGeneralManager { get; set; }

        public EntityWorkerMemberModel()
        {
            WorkerId = string.Empty;
            WorkerName = string.Empty;
            IsBot = false;
            SkillIds = string.Empty;
        }
    }
}
