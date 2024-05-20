using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class ShiftViewModel
    {
        public List<ShiftDTO> Shifts { get; set; }
        public List<ShiftBreakTypeLocalizedDTO> ShiftBreakTypeLocalizeds { get; set; }
        public bool AllowEdit { get; set; }
        public List<ShiftBreakTemplateDTO> ShiftBreakTemplates { get; set; }

        // public List<>

        public ShiftViewModel()
        {
            Shifts = new List<ShiftDTO>();
            ShiftBreakTypeLocalizeds = new List<ShiftBreakTypeLocalizedDTO>();
            ShiftBreakTemplates = new List<ShiftBreakTemplateDTO>();
        }

    }
}
