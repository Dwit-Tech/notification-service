using DwitTech.NotificationService.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.NotificationService.Core.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
