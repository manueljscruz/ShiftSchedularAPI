namespace ShiftSchedularEntity.Models.DataTransferObjects.Billing
{
    public class AvailablePlanDTO
    {
        public Guid SubscriptionPlanDurationPriceId { get; set; }
        public int SubscriptionPlanTypeId { get; set; }
        public string SubscriptionPlanTypeName { get; set; }
        public string SubscriptionPlanTypeDescription { get; set; }
        public int SubscriptionDurationTypeId { get; set; }
        public string SubscriptionDurationTypeName { get; set; }
        public int DurationInDays { get; set; }
        public decimal BasePrice { get; set; }
        public bool ToScale { get; set; }
        public int ScaleRequirement { get; set; }
        public decimal PricePerExtraMember { get; set; }
        public int IncludedGenerations { get; set; }
        public decimal PricePerExtraGeneration { get; set; }
    }
}
