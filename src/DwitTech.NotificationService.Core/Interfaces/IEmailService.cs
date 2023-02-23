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
        Task<bool> SendEmail(string From, string To, string Subject, string Body,string CC="", string BCC="");
       

    }
}
