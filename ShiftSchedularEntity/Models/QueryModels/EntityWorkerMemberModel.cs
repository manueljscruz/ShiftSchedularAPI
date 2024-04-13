namespace ShiftSchedularEntity.Models.QueryModels
{
    public class EntityWorkerMemberModel
    {
        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public bool CanCreateSchedules { get; set; }
        public bool IsOwner { get; set; }
        public string SkillIds { get; set; }

        public EntityWorkerMemberModel()
        {
            WorkerId = string.Empty;
            WorkerName = string.Empty;
            CanCreateSchedules = false;
            IsOwner = false;
            SkillIds = string.Empty;
        }
    }
}
