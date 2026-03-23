using ShiftSchedularEntity.Entities;

namespace ShiftSchedularIL.IServices
{
    public interface IEmailService
    {
        Task SendConfirmEmail(string subject, ApplicationUser user, string strLink);
        Task SendForgotPasswordEmail(string subject, ApplicationUser user, string strLink);
        Task SendInvitationEmail(string toEmail, string displayName, string invitationsLink);
    }
}
