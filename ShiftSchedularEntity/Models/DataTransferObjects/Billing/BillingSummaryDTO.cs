namespace ShiftSchedularEntity.Models.DataTransferObjects.Billing
{
    public class BillingSummaryDTO
    {
        public CurrentSubscriptionPlanDTO CurrentPlan { get; set; }
        public BillingUsageDTO Usage { get; set; }
        public decimal EstimatedAmount { get; set; }
        public List<PaymentMethodSummaryDTO> PaymentMethods { get; set; }

        public BillingSummaryDTO()
        {
            PaymentMethods = new List<PaymentMethodSummaryDTO>();
        }
    }
}
