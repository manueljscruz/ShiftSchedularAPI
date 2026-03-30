using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class EntityHolidaysViewModel
    {
        public bool IsOwner { get; set; }
        public bool AllowEdit { get; set; }
        public Guid? ParentEntityId { get; set; }
        public List<HolidayCatalogLocalizedDTO> HolidayCatalogDTOs { get; set; }
        public List<HolidayBehaviourLocalizedDTO> HolidayBehaviourDTOs { get; set; }
        public PagedList<EntityHolidayDTO> EntityHolidayDTOs { get; set; }
        public List<HolidayTypeLocalizedDTO> HolidayTypeDTOs { get; set; }
    }
}
