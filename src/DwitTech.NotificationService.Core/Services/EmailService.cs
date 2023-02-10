using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
namespace DwitTech.NotificationService.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        //implemented this to test my controller
        public void SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(request.From));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Bcc.Add(MailboxAddress.Parse(request.Bcc));
            email.Cc.Add(MailboxAddress.Parse(request.Cc));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com",587,SecureSocketOptions.StartTls);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
