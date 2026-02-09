using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddEntityHolidayDTO
    {

        [Required]
        public Guid EntityId { get; set; }

        /// <summary>
        /// The HolidayCatalog ID when subscribing to a predefined holiday from the catalog.
        /// Null or not provided when IsCustom is true.
        /// </summary>
        public int? HolidayCatalogId { get; set; }

        [Required]
        public int HolidayBehaviourId { get; set; }

        public bool IsCustom { get; set; }

        public string CustomHolidayName { get; set; }

        public int CustomDay { get; set; }

        public int CustomMonth { get; set; }

        public TimeSpan? OperatingStartTime { get; set; }
        public TimeSpan? OperatingEndTime { get; set; }
        public string Notes { get; set; }

    }
}
