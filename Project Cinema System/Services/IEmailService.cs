namespace Project_Cinema_System.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlMessage);
    }
}