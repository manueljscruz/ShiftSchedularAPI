namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminSubscriptionPlanDurationPriceItemDTO
    {
        public Guid Id { get; set; }
        public int SubscriptionPlanTypeId { get; set; }
        public string SubscriptionPlanTypeName { get; set; }
        public int SubscriptionDurationTypeId { get; set; }
        public string SubscriptionDurationTypeName { get; set; }
        public decimal BasePrice { get; set; }
        public bool ToScale { get; set; }
        public int ScaleRequirement { get; set; }
        public decimal PricePerExtraMember { get; set; }
        public int IncludedGenerations { get; set; }
        public decimal PricePerExtraGeneration { get; set; }
        public bool IsPublicPlan { get; set; }
        public bool IsActive { get; set; }
    }

    public class AdminUpsertSubscriptionPlanDurationPriceDTO
    {
        public Guid Id { get; set; }
        public int SubscriptionPlanTypeId { get; set; }
        public int SubscriptionDurationTypeId { get; set; }
        public decimal BasePrice { get; set; }
        public bool ToScale { get; set; }
        public int ScaleRequirement { get; set; }
        public decimal PricePerExtraMember { get; set; }
        public int IncludedGenerations { get; set; }
        public decimal PricePerExtraGeneration { get; set; }
        public bool IsPublicPlan { get; set; }
        public bool IsActive { get; set; }
    }
}
