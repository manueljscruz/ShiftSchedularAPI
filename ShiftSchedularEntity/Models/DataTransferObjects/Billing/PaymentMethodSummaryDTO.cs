namespace ShiftSchedularEntity.Models.DataTransferObjects.Billing
{
    public class PaymentMethodSummaryDTO
    {
        public Guid PaymentMethodId { get; set; }
        public int PaymentMethodTypeId { get; set; }
        public string PaymentMethodTypeName { get; set; }
        public string CardBrand { get; set; }
        public string LastFourDigits { get; set; }
        public bool IsDefault { get; set; }
    }
}
