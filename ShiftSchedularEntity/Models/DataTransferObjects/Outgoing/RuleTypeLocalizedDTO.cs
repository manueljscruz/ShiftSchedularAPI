namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class RuleTypeLocalizedDTO
    {
        public int RuleTypeId { get; set; }
        public string RuleTypeLocalizedName { get; private set; }
        public bool MultipleSpecification { get; set; }
        public bool IsSpecValuesBoolean { get; set; }
        public string RuleTypeDescriptionLocalized { get; set; }
        public List<BusinessAspectLocalizedDTO> BusinessAspectLocalizedDTOs { get; set; }
        public int OrderNo { get; set; }

        public RuleTypeLocalizedDTO()
        {
            BusinessAspectLocalizedDTOs = new List<BusinessAspectLocalizedDTO>();
        }
    }
}
