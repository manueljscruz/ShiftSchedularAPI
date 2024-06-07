namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityRuleSpecificationDTO
    {
        public string EntityRuleId { get; set; }
        public int SpecificationId { get; set; }
        public int RuleSpecificationValue { get; set; }
        public string AspectReferenceId { get; set; }
    }
}
