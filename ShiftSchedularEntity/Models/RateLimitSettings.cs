namespace ShiftSchedularEntity.Models
{
    public class RateLimitSettings
    {
        public LoginRateLimitSettings Login { get; set; } = new();
        public RegistrationRateLimitSettings Registration { get; set; } = new();
        public ApiRateLimitSettings Api { get; set; } = new();
    }

    public class LoginRateLimitSettings
    {
        public int PermitLimit { get; set; } = 5;
        public int WindowMinutes { get; set; } = 1;
    }

    public class RegistrationRateLimitSettings
    {
        public int TokenLimit { get; set; } = 10;
        public int ReplenishmentHours { get; set; } = 1;
    }

    public class ApiRateLimitSettings
    {
        public int AuthenticatedLimit { get; set; } = 100;
        public int AnonymousLimit { get; set; } = 20;
        public int WindowMinutes { get; set; } = 1;
    }
}
