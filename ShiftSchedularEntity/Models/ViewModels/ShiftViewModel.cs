using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class ShiftViewModel
    {
        public List<ShiftDTO> Shifts { get; set; }
        public List<ShiftBreakTypeLocalizedDTO> ShiftBreakTypeLocalizeds { get; set; }
        public List<EntityShiftRotationDTO> ShiftRotations { get; set; }
        public bool AllowEdit { get; set; }
        public List<ShiftBreakTemplateDTO> ShiftBreakTemplates { get; set; }
        public List<ShiftTemplateDTO> ShiftTemplates { get; set; }
        public Guid? ParentEntityId { get; set; }

        // public List<>

        public ShiftViewModel()
        {
            Shifts = new List<ShiftDTO>();
            ShiftBreakTypeLocalizeds = new List<ShiftBreakTypeLocalizedDTO>();
            ShiftRotations = new List<EntityShiftRotationDTO>();
            ShiftBreakTemplates = new List<ShiftBreakTemplateDTO>();
            ShiftTemplates = new List<ShiftTemplateDTO>();
        }

    }
}
