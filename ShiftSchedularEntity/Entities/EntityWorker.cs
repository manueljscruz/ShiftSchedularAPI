namespace ShiftSchedularEntity.Entities
{
    public class EntityWorker
    {
        #region Properties

        public string EntityId { get; set; }
        public string WorkerId { get; set; }
        public bool ActiveWorkerStatus { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual Worker Worker { get; set; }

        #endregion

    }
}
