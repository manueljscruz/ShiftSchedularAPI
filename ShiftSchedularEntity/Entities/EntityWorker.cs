namespace ShiftSchedularEntity.Entities
{
    public class EntityWorker
    {
        #region Properties

        public string EntityId { get; set; }
        public string WorkerId { get; set; }
        public int SkillId { get; set; }
        public bool ActiveWorkerStatus { get; set; }
        public bool IsOwner { get; set; }
        public bool CanCreateSchedules { get; set; }
        public DateTime DateOfJoin { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual Worker Worker { get; set; }
        public virtual Skill Skill { get; set; }

        #endregion

    }
}
