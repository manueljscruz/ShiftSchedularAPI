namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddEntityRuleSpecificationDTO
    {
        public string EntityRuleId { get; set; }
        public int SpecificationId { get; set; }
        public int RuleSpecificationValue { get; set; }
        public string AspectReferenceId { get; set; }
        public int BusinessAspectId { get; set; }
        public string AspectReferenceId2 { get; set; }
        public int BusinessAspectId2 { get; set; }
        public string LanguageCode { get; set; }
    }
}
