namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddEntityRuleSpecificationDTO
    {
        public string EntityRuleId { get; set; }
        public int RuleSpecificationValue { get; set; }
        public int BusinessAspectId { get; set; }
        public string AspectReferenceId { get; set; }
    }
}
