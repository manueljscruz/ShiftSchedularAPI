using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ConvertBotToUserDTO
    {
        [Required]
        public Guid EntityId { get; set; }

        [Required]
        public Guid UserBotId { get; set; }

        [Required]
        public string ApplicationUserIdTarget { get; set; }
    }
}
