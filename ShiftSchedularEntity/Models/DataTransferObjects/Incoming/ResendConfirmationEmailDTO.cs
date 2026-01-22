using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ResendConfirmationEmailDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
