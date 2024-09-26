namespace ShiftSchedularEntity.Models.QueryModels
{
    public class EntityWorkerMemberModel
    {
        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public bool IsBot { get; set; }
        public bool CanCreateSchedules { get; set; }
        public bool IsOwner { get; set; }
        public DateTime DateOfJoin { get; set; }
        public string SkillIds { get; set; }

        public EntityWorkerMemberModel()
        {
            WorkerId = string.Empty;
            WorkerName = string.Empty;
            IsBot = false;
            CanCreateSchedules = false;
            IsOwner = false;
            SkillIds = string.Empty;
        }
    }
}
