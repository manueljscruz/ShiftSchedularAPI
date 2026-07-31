using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Schedule Generation entity represents a single schedule generation request made within a subscription plan. Only generations with Status = Completed count towards the generation limit of the billing period.
    /// </summary>
    public class ScheduleGeneration
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Schedule Generation.
        /// </summary>
        [Key]
        [Required]
        public Guid ScheduleGenerationId { get; set; }

        /// <summary>
        /// Foreign Key - The unique identifier for the subscription plan under which this generation was requested.
        /// </summary>
        public Guid EntitySubscriptionPlanId { get; set; }

        /// <summary>
        /// Date and time when the generation was requested/started.
        /// </summary>
        public DateTime GeneratedAt { get; set; }

        /// <summary>
        /// Date and time when the generation finished. Null while the generation is still in progress.
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Number of members considered at the time of the generation.
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// Status of the generation (e.g., Queued, Running, Completed, Failed, Cancelled).
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Total duration of the generation, calculated from CompletedAt - GeneratedAt. Null until the generation completes.
        /// </summary>
        public TimeSpan? DurationMs { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Entity subscription plan associated with this generation, providing context for the subscription under which it was requested.
        /// </summary>
        public virtual EntitySubscriptionPlan EntitySubscriptionPlan { get; set; }

        #endregion
    }
}
