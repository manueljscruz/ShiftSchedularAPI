namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddEntityRuleSpecificationDTO
    {
        public Guid EntityRuleId { get; set; }
        public int SpecificationId { get; set; }
        public float RuleSpecificationValue { get; set; }
        public string AspectReferenceId { get; set; }
        public int BusinessAspectId { get; set; }
        public string AspectReferenceId2 { get; set; }
        public int BusinessAspectId2 { get; set; }
    }
}
