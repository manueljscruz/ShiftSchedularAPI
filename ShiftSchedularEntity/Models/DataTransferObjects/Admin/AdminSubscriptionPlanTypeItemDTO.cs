namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminSubscriptionPlanTypeItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<AdminSubscriptionPlanTypeLocalizationValueDTO> Localizations { get; set; } = new();
    }

    public class AdminSubscriptionPlanTypeLocalizationValueDTO
    {
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }
        public string NameDisplayValue { get; set; }
        public string DescriptionDisplayValue { get; set; }
    }

    public class AdminUpsertSubscriptionPlanTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<AdminUpsertSubscriptionPlanTypeLocalizationDTO> Localizations { get; set; } = new();
    }

    public class AdminUpsertSubscriptionPlanTypeLocalizationDTO
    {
        public int LocalizationId { get; set; }
        public string NameDisplayValue { get; set; }
        public string DescriptionDisplayValue { get; set; }
    }
}
