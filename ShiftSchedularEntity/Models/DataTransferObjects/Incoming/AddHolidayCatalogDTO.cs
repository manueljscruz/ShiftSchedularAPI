using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    /// <summary>
    /// Data transfer object for creating a new holiday catalog entry.
    /// Used to define holidays that entities can subscribe to.
    /// </summary>
    public class AddHolidayCatalogDTO
    {
        /// <summary>
        /// The ID of the holiday type (e.g., Public, Religious, Regional).
        /// </summary>
        [Required]
        public int HolidayTypeId { get; set; }

        /// <summary>
        /// The ID of the holiday behaviour (e.g., Closed, Reduced Hours).
        /// </summary>
        [Required]
        public int HolidayBehaviourId { get; set; }

        /// <summary>
        /// The name of the holiday (e.g., "Christmas Day", "New Year's Day").
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string HolidayName { get; set; }

        /// <summary>
        /// A description of the holiday.
        /// </summary>
        [MaxLength(500)]
        public string HolidayDescription { get; set; }

        /// <summary>
        /// The day of the month when the holiday occurs (1-31).
        /// </summary>
        [Required]
        [Range(1, 31)]
        public int RecurrenceDay { get; set; }

        /// <summary>
        /// The month when the holiday occurs (1-12).
        /// </summary>
        [Required]
        [Range(1, 12)]
        public int RecurrenceMonth { get; set; }

        /// <summary>
        /// Indicates whether the holiday recurs every year.
        /// </summary>
        public bool IsRecurring { get; set; } = true;

        /// <summary>
        /// Indicates whether the holiday catalog entry is active.
        /// </summary>
        public int IsActive { get; set; } = 1;
    }
}
