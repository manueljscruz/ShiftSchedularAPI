using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
