using ShiftSchedularEntity.Entities;

namespace ShiftSchedularIL.IServices
{
    public interface IEmailService
    {
        Task SendConfirmEmail(Worker worker);
        // Task SendEmail(List<string> to, List<string> cc, string subject, string body);
    }
}
