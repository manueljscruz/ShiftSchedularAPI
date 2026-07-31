namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminSubscriptionDurationTypeItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationInDays { get; set; }
        public bool AppliesPromo { get; set; }
        public decimal PromoPercentage { get; set; }
        public List<AdminSubscriptionDurationTypeLocalizationValueDTO> Localizations { get; set; } = new();
    }

    public class AdminSubscriptionDurationTypeLocalizationValueDTO
    {
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }
        public string DisplayValue { get; set; }
    }

    public class AdminUpsertSubscriptionDurationTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationInDays { get; set; }
        public bool AppliesPromo { get; set; }
        public decimal PromoPercentage { get; set; }
        public List<AdminUpsertSubscriptionDurationTypeLocalizationDTO> Localizations { get; set; } = new();
    }

    public class AdminUpsertSubscriptionDurationTypeLocalizationDTO
    {
        public int LocalizationId { get; set; }
        public string DisplayValue { get; set; }
    }
}
