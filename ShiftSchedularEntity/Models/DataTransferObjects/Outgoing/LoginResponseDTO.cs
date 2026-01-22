namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public TokenResponseDTO TokenResponseDTO { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
