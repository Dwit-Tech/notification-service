using DwitTech.NotificationService.Core.Interfaces;
using Google.Apis.Gmail.v1;
using Microsoft.Extensions.Configuration;
namespace DwitTech.NotificationService.Core.Services
{
    public class EmailService : IEmailService
    {
        public bool SendEmail(string From, string To, string Subject, string Body, string CC = "", string BCC = "")
        {
            throw new NotImplementedException();
        }
    }
}
