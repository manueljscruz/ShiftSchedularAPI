namespace ShiftSchedularEntity.Models.DataTransferObjects
{
    public class TokenModelDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public TokenModelDTO()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
        }
    }
}
