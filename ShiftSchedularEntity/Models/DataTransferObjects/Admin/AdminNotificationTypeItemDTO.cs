namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminNotificationTypeItemDTO
    {
        public int Id { get; set; }
        public string NotificationTypeCode { get; set; }
        public List<AdminNotificationTypeLocalizationValueDTO> Localizations { get; set; } = new();
    }

    public class AdminNotificationTypeLocalizationValueDTO
    {
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }
        public string DisplayValue { get; set; }
        public string MessageTemplate { get; set; }
    }

    public class AdminUpsertNotificationTypeDTO
    {
        public int Id { get; set; }
        public string NotificationTypeCode { get; set; }
        public List<AdminUpsertNotificationTypeLocalizationDTO> Localizations { get; set; } = new();
    }

    public class AdminUpsertNotificationTypeLocalizationDTO
    {
        public int LocalizationId { get; set; }
        public string DisplayValue { get; set; }
        public string MessageTemplate { get; set; }
    }
}
