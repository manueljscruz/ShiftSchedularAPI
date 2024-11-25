namespace ShiftSchedularIL.IServices
{
    public interface ILoggerService
    {
        Task LogError(string message);
        Task LogInfo(string message);
        Task LogDebug(string message);
        Task LogWarning(string message);
    }
}
