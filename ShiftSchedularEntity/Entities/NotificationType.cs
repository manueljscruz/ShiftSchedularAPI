using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class NotificationType
    {
        [Key]
        public int NotificationTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string NotificationTypeCode { get; set; }

        // Navigation properties
        public virtual ICollection<NotificationTypeLocalization> NotificationTypeLocalizations { get; set; }
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
    }
}
