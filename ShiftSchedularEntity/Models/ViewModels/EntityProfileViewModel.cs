using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class EntityProfileViewModel
    {
        public EntityDTO EntityDTO { get; set; }
        public bool AllowEdit { get; set; }
        public List<EntityTypeLocalizedDTO> EntityTypeLocalizeds { get; set; }

        public EntityProfileViewModel()
        {
            EntityTypeLocalizeds = new List<EntityTypeLocalizedDTO>();
        }
    }
}
