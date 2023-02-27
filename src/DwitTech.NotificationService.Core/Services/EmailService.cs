using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Data.Entities;
using DwitTech.NotificationService.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using System.Net.Mail;

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

         try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(emailDto.To);
                mail.From = new MailAddress(_config["GmailInfo:Email"]);
                mail.Subject = emailDto.Subject;
                mail.Body = emailDto.Body;
                mail.IsBodyHtml = true;

                using (var SmtpClient = new SmtpClient())
                {
                    SmtpClient.Port = Int32.Parse(_config["GmailInfo:Port"]);
                    SmtpClient.EnableSsl = true;
                    SmtpClient.UseDefaultCredentials = false;
                    SmtpClient.Host = _config["GmailInfo:Host"];
                    SmtpClient.Credentials = new System.Net.NetworkCredential(_config["GmailInfo:Email"], _config["GmailInfo:AppPassword"]);
                    await SmtpClient.SendMailAsync(mail);
                }

                
                await _emailRepo.UpdateEmailStatus(emailModel, true);
                _logger.LogInformation(2, "At this point the email status gets updated after successfully sending the email");
                return true;
            }
            catch (SmtpException ex)
            {

                await _emailRepo.UpdateEmailStatus(emailModel, false);
                _logger.LogError(3, $"This is the log that is called if for any reason the email sending process fails due to {ex.Message}");
                return false;   
            }


             
        }

        
    }
}
