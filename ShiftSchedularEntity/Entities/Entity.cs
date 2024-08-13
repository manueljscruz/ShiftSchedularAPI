namespace ShiftSchedularEntity.Entities
{
    public class Entity
    {
        #region Properties

        public string EntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityDescription { get; set; }
        public int EntityTypeId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual EntityType EntityType { get; set; }
        public virtual ICollection<EntityWorker> EntityWorkers { get; set; }
        public virtual ICollection<EntityWorkerInvitation> EntityWorkerInvitations { get; set; }
        public virtual ICollection<Shift> EntityShifts { get; set; }
        public virtual ICollection<EntityRule> EntityRules { get; set; }
        public virtual ICollection<EntityWorkerAbsence> EntityWorkerAbsences { get; set; }
        //public virtual ICollection<ScheduleEntry> ScheduleEntries { get; set; }
        
        #endregion
    }
}
