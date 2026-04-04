using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class NotificationTypeLocalization
    {
        [Required]
        public int NotificationTypeId { get; set; }

        [Required]
        public int LocalizationId { get; set; }

        [Required]
        public string NotificationTypeDisplayValue { get; set; }

        /// <summary>
        /// Localized message template. Use {0} as a single optional placeholder
        /// for runtime context data (e.g. entity name).
        /// Example: "You have been invited to join {0}."
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string MessageTemplate { get; set; }

        // Navigation property
        public virtual NotificationType NotificationType { get; set; }
        public virtual Localization Localization { get; set; }
    }
}
