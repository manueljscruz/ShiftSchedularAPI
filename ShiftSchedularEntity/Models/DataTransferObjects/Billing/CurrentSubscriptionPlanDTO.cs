namespace ShiftSchedularEntity.Models.DataTransferObjects.Billing
{
    public class CurrentSubscriptionPlanDTO
    {
        public Guid EntitySubscriptionPlanId { get; set; }
        public int SubscriptionPlanTypeId { get; set; }
        public string SubscriptionPlanTypeName { get; set; }
        public int SubscriptionDurationTypeId { get; set; }
        public string SubscriptionDurationTypeName { get; set; }
        public decimal BasePrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
