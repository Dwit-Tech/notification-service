using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Data.Entities;
using DwitTech.NotificationService.Data.Repository;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Sockets;

namespace DwitTech.NotificationService.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailRepo _emailRepo;
        private readonly ILogger<EmailService> _logger;
        
        public EmailService(IConfiguration config, IEmailRepo emailRepo, ILogger<EmailService> logger)
        {
            _config = config;
            _emailRepo = emailRepo;
            _logger = logger;
            
        }

        public async Task<bool> SendEmail(EmailDto emailDto)
        {
            
            var emailModel = new Email { From = emailDto.From, To = emailDto.To, Subject = emailDto.Subject, Body = emailDto.Body, Cc = emailDto.Cc , Bcc = emailDto.Bcc  };

            await _emailRepo.CreateEmail(emailModel);
            _logger.LogInformation(1, "The email has been inserted into the database");


            var mail = new MimeMessage();
            mail.To.Add(MailboxAddress.Parse(emailDto.To));
            mail.From.Add(MailboxAddress.Parse(_config["GmailInfo:Email"]));
            mail.Subject = emailDto.Subject;
            mail.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailDto.Body };

           
            var client = GenerateSmtpClient();
           
            try
            {
                await client.SendAsync(mail);
                client.Disconnect(true);
                await _emailRepo.UpdateEmailStatus(emailModel, true);
                _logger.LogInformation(2, "At this point the email status gets updated after successfully sending the email");
                return true;
            }
            catch (Exception ex) when (ex is SocketException)
            {

                await _emailRepo.UpdateEmailStatus(emailModel, false);
                _logger.LogError(3, $"This is the log that is called if for any reason the email sending process fails due to {ex.Message}");
                return false;   
            }


             
        }
        
        private SmtpClient GenerateSmtpClient()
        {
            var smtpClient = new SmtpClient();
            smtpClient.Connect(_config["GmailInfo:Host"], Convert.ToInt32(_config["GmailInfo:Port"]), SecureSocketOptions.StartTls);
            smtpClient.Authenticate(_config["GmailInfo:Email"], _config["GmailInfo:AppPassword"]);
            return smtpClient;
        }
        
    }
}
