using MailKit.Net.Smtp;
using MimeKit;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularIL.IServices;

namespace ShiftSchedularIL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        #region Send Confirm Email

        public async Task SendConfirmEmail(string subject, ApplicationUser user, string strLink)
        {
            string body = EmailMessages.CONFIRM_EMAIL_EMAIL;

            body = body.Replace("[UserName]", user.DisplayName)
                       .Replace("[ConfirmationLink]", strLink)
                       .Replace("[Year]", DateTime.UtcNow.Year.ToString());

            await SendEmail(new List<string> { user.Email }, new List<string>(), subject, body);
        }

        #endregion

        #region Send Forgot Password Email

        public async Task SendForgotPasswordEmail(string subject, ApplicationUser user, string strLink)
        {
            string body = EmailMessages.POST_FORGOT_PASSWORD_EMAIL;

            body = body.Replace("[UserName]", user.DisplayName)
                .Replace("[ResetPasswordLink]", strLink)
                .Replace("[Year]", DateTime.UtcNow.Year.ToString());


            await SendEmail(new List<string> { user.Email }, new List<string>(), subject, body);

        }

        #endregion

        #region Send Email

        private async Task SendEmail(List<string> to, List<string> cc, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Shift Scheduler - Support", _emailSettings.SenderEmail));
                foreach (string strEmail in to)
                    email.To.Add(MailboxAddress.Parse(strEmail));
                foreach (string strEmail in cc)
                    email.Cc.Add(MailboxAddress.Parse(strEmail));
                email.Subject = subject;

                email.Body = new TextPart("html") { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SenderUser, _emailSettings.SenderPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        #endregion
    }
}
