using MailKit.Net.Smtp;
using MimeKit;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularIL.IServices;

namespace ShiftSchedularIL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings = new EmailSettings
        {
            SmtpServer = "pro.eu.turbo-smtp.com",
            Port = 587,
            SenderEmail = "manueljscruz93@gmail.com",
            SenderUser = "manueljscruz93@gmail.com",
            SenderPassword = "Hi8Fb8zX"
        };
        private readonly string _baseUrl = "http://localhost:4200/";

        //public EmailService(EmailSettings emailSettings, string baseUrl)
        //{
        //    _emailSettings = emailSettings;
        //    _baseUrl = baseUrl;
        //}
        public EmailService()
        {
            
        }

        public async Task SendConfirmEmail(Worker worker)
        {
            string body = EmailMessages.POST_REGISTRATION_EMAIL;
            body = body.Replace("[Name]", worker.WorkerName)
                       .Replace("[Url]", _baseUrl)
                       .Replace("[WorkerId]", worker.WorkerId);

            await SendEmail(new List<string> { worker.Email }, new List<string>(), "", body);
        }

        private async Task SendEmail(List<string> to, List<string> cc, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Shift Schedular - Support", _emailSettings.SenderEmail));
                email.Subject = "Confirm Email Address";
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
    }
}
