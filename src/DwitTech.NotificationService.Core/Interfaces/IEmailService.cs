using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Core.Interfaces
{
    public interface IEmailService
    {
        bool SendEmail(string From, string To, string Subject, string Body,string CC="", string BCC="");
       // bool SaveEmail(EmailDto email);
        Email FindEmail(string email);
        void UpdateEmailStatus(EmailDto email, bool status);
        EmailDto CreateEmail(EmailDto email);
    }
}
