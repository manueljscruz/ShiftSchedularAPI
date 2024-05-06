namespace ShiftSchedularEntity.Models.DataTransferObjects
{
    public class EntityWorkerMemberDTO
    {
        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public bool CanCreateSchedules { get; set; }
        public bool IsOwner { get; set; }
        public DateTime DateOfJoin { get; set; }
        public List<SkillLocalizedDTO> SkillSet { get; set; }

        public EntityWorkerMemberDTO()
        {
            SkillSet = new List<SkillLocalizedDTO>();
        }
    }
}
