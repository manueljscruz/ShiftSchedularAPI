namespace ShiftSchedularEntity.Models.DataTransferObjects.Billing
{
    public class SubscribeRequestDTO
    {
        public Guid SubscriptionPlanDurationPriceId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string CouponCode { get; set; }
    }
}
