namespace ShiftSchedularEntity.Models.DataTransferObjects.Billing
{
    public class BillingUsageDTO
    {
        public int MembersUsed { get; set; }
        public bool ToScale { get; set; }
        public int? MembersIncluded { get; set; }
        public int GenerationsUsed { get; set; }
        public int GenerationsIncluded { get; set; }
    }
}
