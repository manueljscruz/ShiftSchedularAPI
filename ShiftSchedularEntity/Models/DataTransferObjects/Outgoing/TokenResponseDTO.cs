namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class TokenResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public TokenResponseDTO(string token, string refreshToken, DateTime expiration)
        {
            Token = token;
            RefreshToken = refreshToken;
            Expiration = expiration;
        }
    }
}
