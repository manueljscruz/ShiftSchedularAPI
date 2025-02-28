namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddEntityRuleDTO
    {
        public int RuleTypeId { get; set; }
        public string RuleTypeDescription { get; set; }
        public Guid EntityId { get; set; }
        public List<AddEntityRuleSpecificationDTO> EntityRuleSpecifications { get; set; }
    }
}
