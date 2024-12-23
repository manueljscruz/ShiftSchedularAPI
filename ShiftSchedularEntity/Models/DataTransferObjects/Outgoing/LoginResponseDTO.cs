namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class LoginResponseDTO
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
