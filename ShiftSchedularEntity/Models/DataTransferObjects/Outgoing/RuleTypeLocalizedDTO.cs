namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class RuleTypeLocalizedDTO
    {
        public int RuleTypeId { get; set; }
        public string RuleTypeLocalizedName { get; private set; }
        public List<BusinessAspectLocalizedDTO> BusinessAspectLocalizedDTOs { get; set; }

        public RuleTypeLocalizedDTO()
        {
            BusinessAspectLocalizedDTOs = new List<BusinessAspectLocalizedDTO>();
        }
    }
}
