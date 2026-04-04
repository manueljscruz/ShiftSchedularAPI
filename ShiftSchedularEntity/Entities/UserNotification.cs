using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Represents a single in-app notification delivered to a specific user.
    /// Does NOT extend BaseEntity — notifications are append-only records
    /// that should not be affected by the global soft-delete query filter.
    /// </summary>
    public class UserNotification
    {
        [Key]
        public Guid UserNotificationId { get; set; }

        /// <summary>
        /// The recipient user. FK → AspNetUsers.Id
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// FK → NotificationType.NotificationTypeId
        /// </summary>
        [Required]
        public int NotificationTypeId { get; set; }

        /// <summary>
        /// Optional runtime value substituted into the {0} placeholder
        /// of the localized message template (e.g. entity name, shift name).
        /// </summary>
        [MaxLength(200)]
        public string? ContextData { get; set; }

        /// <summary>
        /// Optional reference to the related domain object (Guid as string).
        /// E.g. entityId, absenceId, scheduleEntryId.
        /// </summary>
        [MaxLength(100)]
        public string? RelatedEntityId { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// UTC timestamp of when this notification was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual NotificationType NotificationType { get; set; }
    }
}
