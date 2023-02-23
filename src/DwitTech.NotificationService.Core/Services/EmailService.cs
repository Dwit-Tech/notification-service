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

        public async Task<bool> SendEmail(string From, string To, string Subject, string Body, string Cc = "", string Bcc = "")
        {

            var emailModel = new Email { From = From, To = To, Subject = Subject, Body = Body, Cc = Cc , Bcc = Bcc  };

            _emailRepo.CreateEmail(emailModel);
            _logger.LogInformation(1, "The email has been inserted into the database");

         try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(To);
                mail.From = new MailAddress(_config["GmailInfo:Email"]);
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Port = Int32.Parse(_config["GmailInfo:Port"]);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Host = _config["GmailInfo:Host"];
                smtp.Credentials = new System.Net.NetworkCredential(_config["GmailInfo:Email"], _config["GmailInfo:AppPassword"]);
                smtp.Send(mail);

                _emailRepo.UpdateEmailStatus(emailModel, true);
                _logger.LogInformation(2, "At this point the email status gets updated after successfully sending the email");

                return true;
            }
            catch (Exception ex)
            {

                
                _emailRepo.UpdateEmailStatus(emailModel, false);
                _logger.LogError(3, "This is the log that is called if for any reason the email sending process fails");
                return false;   
            }


             
        }

        
    }
}
