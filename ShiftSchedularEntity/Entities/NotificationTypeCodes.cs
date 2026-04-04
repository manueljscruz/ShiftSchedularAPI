namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// String constants for NotificationType.NotificationTypeCode values.
    /// Use these instead of magic strings when creating notifications.
    /// </summary>
    public static class NotificationTypeCodes
    {
        public const string InvitationReceived = "InvitationReceived";
        public const string AbsenceApproved    = "AbsenceApproved";
        public const string AbsenceDeclined    = "AbsenceDeclined";
        public const string ScheduleAssigned   = "ScheduleAssigned";
        public const string ExitDateSet        = "ExitDateSet";
    }
}
