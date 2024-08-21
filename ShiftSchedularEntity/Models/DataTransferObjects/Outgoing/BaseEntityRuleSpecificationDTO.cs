namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class BaseEntityRuleSpecificationDTO
    {
        public int BaseEntityRuleId { get; set; }
        public int SpecificationId { get; set; }
        public int SpecificationValue { get; set; }
        public bool IsSpecValueBoolean { get; set; }
        public int BusinessAspectId { get; set; }

        public BaseEntityRuleSpecificationDTO()
        {
            
        }
    }
}
