namespace ShiftSchedularEntity.Models
{
    public class BaseResponse<TResult>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TResult Result { get; set; }
        public bool NotFound { get; set; }
        public bool Forbidden { get; set; }

        public BaseResponse()
        {
            Success = false;
            Message = string.Empty;
            NotFound = false;
            Forbidden = false;
        }
    }
}
