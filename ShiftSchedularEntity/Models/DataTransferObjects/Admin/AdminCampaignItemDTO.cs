namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminCampaignItemDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal PromotionPercent { get; set; }
        public bool IsActive { get; set; }
        public int MaxRedemptions { get; set; }
        public int RedemptionCount { get; set; }
        public string CouponCode { get; set; }
        public List<Guid> EligibleSubscriptionPlanDurationPriceIds { get; set; } = new();
        public List<AdminCampaignLocalizationValueDTO> Localizations { get; set; } = new();
    }

    public class AdminCampaignLocalizationValueDTO
    {
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }
        public string NameDisplayValue { get; set; }
        public string DescriptionDisplayValue { get; set; }
    }

    public class AdminUpsertCampaignDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal PromotionPercent { get; set; }
        public bool IsActive { get; set; }
        public int MaxRedemptions { get; set; }
        public string CouponCode { get; set; }
        public List<Guid> EligibleSubscriptionPlanDurationPriceIds { get; set; } = new();
        public List<AdminUpsertCampaignLocalizationDTO> Localizations { get; set; } = new();
    }

    public class AdminUpsertCampaignLocalizationDTO
    {
        public int LocalizationId { get; set; }
        public string NameDisplayValue { get; set; }
        public string DescriptionDisplayValue { get; set; }
    }
}
