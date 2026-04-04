namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    /// <summary>
    /// Represents a single in-app notification returned to the client.
    /// The Message is pre-formatted (template + context data substituted server-side).
    /// </summary>
    public class UserNotificationDTO
    {
        public Guid Id { get; set; }
        public string NotificationTypeCode { get; set; }
        public string Message { get; set; }
        public string? RelatedEntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
