using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class EntityRuleViewModel
    {
        public List<EntityRuleDTO> EntityRules { get; set; }
        public bool AllowEdit { get; set; }
        public List<BusinessAspectLocalizedDTO> BusinessAspectsLocalizeds { get; set; }
        public List<RuleTypeLocalizedDTO> RuleTypeLocalizeds { get; set; }
        // Add Templates here

        public EntityRuleViewModel()
        {
            EntityRules = new List<EntityRuleDTO>();
            AllowEdit = false;
            BusinessAspectsLocalizeds = new List<BusinessAspectLocalizedDTO>();
            RuleTypeLocalizeds = new List<RuleTypeLocalizedDTO>();
        }
    }
}
