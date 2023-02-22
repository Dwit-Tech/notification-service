using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.Data.Entities;
using DwitTech.NotificationService.Data.Repository;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Net;
using System.Net.Mail;

namespace DwitTech.NotificationService.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IEmailRepo _emailRepo;
        public EmailService(IConfiguration config, IEmailRepo emailRepo)
        {
            _config = config;
            _emailRepo = emailRepo; 
        }

        public bool SendEmail(string From, string To, string Subject, string Body, string Cc = "", string Bcc = "")
        {

            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(To);
                mail.From = new MailAddress(_config["GmailInfo:Email"]);
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Host = _config["GmailInfo:Host"];
                smtp.Credentials = new System.Net.NetworkCredential(_config["GmailInfo:Email"], _config["GmailInfo:AppPassword"]);
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {

                logger.Error(ex);
                return false;   
            }


             
        }

        //public bool SaveEmail(EmailDto email)
        //{
            
        //}

        public Email FindEmail(string email)
        {

            var findEmail =  _emailRepo.FindEmail(email);

            if(findEmail != null)
            {
                return findEmail;
            }

            return null;
            
        }

        public void UpdateEmailStatus(EmailDto email, bool status)
        {
            var emailToBeSearched = email.To;
            var findMailResult = this.FindEmail(emailToBeSearched);
            _emailRepo.UpdateEmailStatus(findMailResult, status);
        }

        public EmailDto CreateEmail(EmailDto email)
        {
            var read = new Email {
                From = email.From,
                To = email.To,
                Subject = email.Subject,
                Body = email.Body,
                Status = EmailStatus.Pending,
                Cc = email.Cc,
                Bcc = email.Bcc
            };
            _emailRepo.CreateEmail(read);
            _emailRepo.SaveChanges();
            return email;

        }
    }
}
