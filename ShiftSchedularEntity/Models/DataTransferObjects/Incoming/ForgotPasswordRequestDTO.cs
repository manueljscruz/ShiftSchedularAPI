using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ForgotPasswordRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
