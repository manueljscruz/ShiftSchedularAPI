using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class Worker
    {
        #region Properties
        
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public string WorkerId { get; set; }

        /// <summary>
        /// Name of the Worker
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string WorkerName { get; set; }

        /// <summary>
        /// Assigned Gender Id
        /// </summary>
        [Required]
        [Range(1,3)]
        public int GenderId { get; set; }

        /// <summary>
        /// Email of the Worker
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// Password of the worker
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        /// <summary>
        /// Flag that indicates that the worker account is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Flag that indicates that the worker instance is a bot
        /// </summary>
        public bool IsBot { get; set; }

        /// <summary>
        /// Flag that indicates that the worker has confirmed its email
        /// </summary>
        public bool EmailConfirmed { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Gender Gender { get; set; }
        public virtual ICollection<EntityWorker> EntityWorkers { get; set; }
        public virtual ICollection<EntityWorkerInvitation> EntityWorkerInvitations { get; set; }
        public virtual ICollection<EntityWorkerAbsence> EntityWorkerAbsences { get; set; }
        public virtual ICollection<ScheduleEntryWorkers> ScheduleEntryWorkers { get; set; }

        #endregion
    }
}
