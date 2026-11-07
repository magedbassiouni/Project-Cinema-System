using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Project_Cinema_System.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlMessage)
        {
            var message = new MimeMessage();

            var senderEmail =
                _configuration["EmailSettings:SenderEmail"];

            var senderName =
                _configuration["EmailSettings:SenderName"];

            message.From.Add(
                new MailboxAddress(senderName, senderEmail));

            message.To.Add(
                MailboxAddress.Parse(toEmail));

            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = htmlMessage
            }.ToMessageBody();

            using var smtpClient = new SmtpClient();

            var smtpServer =
                _configuration["EmailSettings:SmtpServer"];

            var smtpPort =
                _configuration.GetValue<int>("EmailSettings:Port");

            var username =
                _configuration["EmailSettings:Username"];

            var password =
                _configuration["EmailSettings:Password"];

            await smtpClient.ConnectAsync(
                smtpServer,
                smtpPort,
                SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(
                username,
                password);

            await smtpClient.SendAsync(message);

            await smtpClient.DisconnectAsync(true);
        }
    }
}