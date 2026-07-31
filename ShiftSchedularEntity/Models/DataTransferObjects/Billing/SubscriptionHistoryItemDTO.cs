namespace ShiftSchedularEntity.Models.DataTransferObjects.Billing
{
    public class SubscriptionHistoryItemDTO
    {
        public Guid EntitySubscriptionPlanId { get; set; }
        public string SubscriptionPlanTypeName { get; set; }
        public string SubscriptionDurationTypeName { get; set; }
        public decimal BasePrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
